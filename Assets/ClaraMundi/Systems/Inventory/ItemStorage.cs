using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ClaraMundi
{
    public class ItemStorage : NetworkBehaviour
    {
        public static event Action<string, string, ItemStorage> OnInitialize;

        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        [ShowInInspector]
        public readonly SyncDictionary<int, ItemInstance> PrivateItems = new();

        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        public readonly SyncList<string> HeldItemIds = new();

        [SyncObject] public readonly SyncDictionary<int, ItemInstance> PublicItems = new();

        public Entity OwnerEntity;
        [SyncVar] public string StorageId = "inventory";
        public static ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;

        public int Capacity = 30;

        public List<ItemInstance> StartingItems = new();

        [SyncVar] public bool isPublicStorage;

        private void Awake()
        {
            OwnerEntity = GetComponentInParent<Entity>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            StorageId = StorageId ?? StringUtils.UniqueId();

            ItemManager.Instance.RegisterStorage(this);
            foreach (var item in StartingItems)
                AddItem(item.ItemId, item.Quantity, true);

            OnInitialize?.Invoke(OwnerEntity.entityId, StorageId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            // Keep shortcut reference to these values so we can have easy reference to item storage for entities in range
            ItemManager.Instance.RegisterStorage(this);
            OnInitialize?.Invoke(OwnerEntity.entityId, StorageId, this);
        }

        private void OnDestroy()
        {
            ItemManager.Instance.RemoveStorage(this);
        }

        private void Add(ItemInstance item)
        {
            UpdateItemInstance(item);
        }

        void Remove(ItemInstance item)
        {
            item.Quantity = 0;
            UpdateItemInstance(item);
        }

        public ItemInstance GetItemInstance(int itemInstanceId, bool mustNotBeEquipped = false)
        {
            PublicItems.TryGetValue(itemInstanceId, out var instance);
            if (instance == null)
                PrivateItems.TryGetValue(itemInstanceId, out instance);
            return mustNotBeEquipped && instance is { IsEquipped: true } ? null : instance;
        }

        public ItemInstance AddItem(string itemId, int quantity, bool forceNewStack = false)
        {
            if (!IsServer) return null;
            Item item = ItemRepo.GetItem(itemId);
            if (item == null) return null;
            if (!CanAdd(item.ItemId, quantity, forceNewStack)) return null;
            var instance = GetInstanceByItemId(item.ItemId);
            if (!forceNewStack && (instance != null && item.Stackable))
            {
                instance.Quantity += quantity;
                UpdateItemInstance(instance);
                return instance;
            }

            var newInstance = new ItemInstance()
            {
                ItemInstanceId = (ItemManager.Instance.ItemsByInstanceId.Count + 1),
                CharacterId = OwnerEntity.entityId,
                ItemId = item.ItemId,
                Quantity = quantity
            };
            Debug.Log(newInstance.ItemInstanceId);
            Add(newInstance);
            return newInstance;
        }

        public bool CanAdd(string itemId, int quantity, bool forceNewStack = false)
        {
            if (forceNewStack && PrivateItems.Count >= Capacity) return false;
            var item = ItemRepo.GetItem(itemId);
            var instance = GetInstanceByItemId(itemId);
            if (item.Unique)
                if (instance != null)
                    return false;
            if (item.Stackable && !forceNewStack && instance != null) return true;
            return PrivateItems.Count < Capacity;
        }

        public ItemInstance GetInstanceByItemId(string itemId, bool mustNotBeEquipped = false)
        {
            return (from kvp in GetVisibleItems()
                where kvp.Value.ItemId == itemId && (!mustNotBeEquipped || !kvp.Value.IsEquipped)
                select kvp.Value).FirstOrDefault();
        }

        public bool CanDrop(string itemId, int quantity)
        {
            var item = ItemRepo.GetItem(itemId);
            if (item == null) return false;
            if (!item.Droppable) return false;
            return !HasQuantity(itemId, quantity, true);
        }

        public bool CanTrade(string itemId)
        {
            var item = ItemRepo.GetItem(itemId);
            if (item == null || item.Untradeable) return false;
            return GetInstanceByItemId(itemId, true) != null;
        }

        public bool CanTradeInstance(int itemInstanceId)
        {
            var instance = GetItemInstance(itemInstanceId, true);
            if (instance == null) return false;
            var item = ItemRepo.GetItem(instance.ItemId);
            return item != null && !item.Untradeable;
        }

        public bool HasQuantity(string itemId, int quantity, bool mustNotBeEquipped = false)
        {
            return quantity <= QuantityOf(itemId, mustNotBeEquipped);
        }

        public SyncDictionary<int, ItemInstance> GetVisibleItems()
        {
            if (PublicItems.Count > 0 && PrivateItems.Count == 0)
                return PublicItems;
            return PrivateItems;
        }

        public int QuantityOf(string itemId, bool mustNotBeEquipped = false)
        {
            return GetVisibleItems().Aggregate(0, (acc, element) =>
            {
                if (element.Value.IsEquipped && mustNotBeEquipped) return acc;
                if (element.Value.ItemId == itemId) return acc + element.Value.Quantity;
                return acc;
            });
        }

        public bool RemoveItemInstance(int itemInstanceId, int quantity, bool allowPullingFromOthers = false)
        {
            if (!IsServer) return false;
            var instance = GetItemInstance(itemInstanceId, true);
            if (instance == null)
                return false;
            if (!allowPullingFromOthers && instance.Quantity < quantity)
                return false;
            if (allowPullingFromOthers && !HasQuantity(instance.ItemId, quantity, true))
                return false;
            if (instance.Quantity <= quantity)
                return RemoveInstance(instance, quantity);
            instance.Quantity -= quantity;
            UpdateItemInstance(instance);
            return true;
        }

        public bool RemoveItem(string itemId, int quantity, bool validated = false)
        {
            if (!IsServer) return false;
            var instance = GetInstanceByItemId(itemId, true);

            if (!validated && !HasQuantity(instance.ItemId, quantity, true)) return false;
            if (instance == null) return false;
            if (instance.Quantity <= quantity)
                return RemoveInstance(instance, quantity);
            instance.Quantity -= quantity;
            UpdateItemInstance(instance);
            return true;
        }

        bool RemoveInstance(ItemInstance instance, int quantity)
        {
            int diff = quantity - instance.Quantity;
            int newQuantity = quantity - diff;
            Remove(instance);
            return newQuantity <= 0 || RemoveItem(instance.ItemId, newQuantity, true);
        }

        public ItemInstance SplitStack(int itemInstanceId, int quantity)
        {
            var instance = GetItemInstance(itemInstanceId);
            if (instance != null && RemoveItemInstance(instance.ItemInstanceId, quantity))
                return AddItem(instance.ItemId, quantity, true);
            return null;
        }

        public void UpdateItemInstance(ItemInstance instance, bool isPublic = false)
        {
            if (!IsServer) return;
            // 0 is an invalid ID
            if (instance.ItemInstanceId == 0) return;
            var dictionary = PrivateItems;
            if (isPublicStorage)
                dictionary = PublicItems;
            dictionary[instance.ItemInstanceId] = new ItemInstance
            {
                CharacterId = OwnerEntity.entityId,
                ItemId = instance.ItemId,
                ItemInstanceId = instance.ItemInstanceId,
                Quantity = instance.Quantity,
                StorageId = StorageId,
                IsEquipped = instance.IsEquipped
            };
            if (!HeldItemIds.Contains(instance.ItemId))
                HeldItemIds.Add(instance.ItemId);
            ItemManager.Instance.ItemsByInstanceId[instance.ItemInstanceId] = dictionary[instance.ItemInstanceId];
            if (instance.Quantity != 0) return;
            dictionary.Remove(instance.ItemInstanceId);

            if (HeldItemIds.Contains(instance.ItemId))
                HeldItemIds.Remove(instance.ItemId);
            ItemManager.Instance.ItemsByInstanceId.Remove(instance.ItemInstanceId);
        }
    }
}