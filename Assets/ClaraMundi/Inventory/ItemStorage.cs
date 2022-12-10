﻿using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class ItemStorage : NetworkBehaviour
    {
        public static event Action<string, string, ItemStorage> OnInitialize;

        [SyncObject(ReadPermissions = ReadPermission.OwnerOnly)]
        public readonly SyncDictionary<string, ItemInstance> PrivateItems = new();

        [SyncObject] public readonly SyncDictionary<string, ItemInstance> PublicItems = new();
        public string Name = "Inventory";
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
            StorageId = StorageId ?? Guid.NewGuid().ToString();

            ItemManager.Instance.RegisterStorage(this);
            for (int index = 0; index < StartingItems.Count; index++)
            {
                var item = StartingItems[index];
                // regenerate ID to avoid collision
                item.ItemInstanceId = Guid.NewGuid().ToString();
                Add(item);
            }

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

        public ItemInstance GetItemInstance(string itemInstanceId)
        {
            PublicItems.TryGetValue(itemInstanceId ?? "", out var instance);
            if (instance == null)
                PrivateItems.TryGetValue(itemInstanceId ?? "", out instance);
            return instance;
        }

        public ItemInstance AddItem(string itemId, int quantity, bool forceNewStack = false)
        {
            return AddItem(ItemRepo.GetItem(itemId), quantity, forceNewStack);
        }

        public ItemInstance AddItem(Item item, int quantity, bool forceNewStack = false)
        {
            if (!IsServer) return null;
            if (item == null) return null;
            if (!CanAdd(item.ItemId, quantity, forceNewStack)) return null;
            var instance = GetInstanceByItemId(item.ItemId);
            if (!forceNewStack && (instance != null && item.Stackable))
            {
                instance.Quantity += quantity;
                UpdateItemInstance(instance);
                return instance;
            }

            var newInstance = new ItemInstance
            {
                OwnerId = OwnerEntity.entityId,
                ItemId = item.ItemId,
                Quantity = quantity
            };
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

        public ItemInstance GetInstanceByItemId(string itemId)
        {
            return (from kvp in GetVisibleItems() where kvp.Value.ItemId == itemId select kvp.Value).FirstOrDefault();
        }

        public bool CanDrop(string itemId, int quantity)
        {
            var item = ItemRepo.GetItem(itemId);
            if (item == null) return false;
            if (!item.Droppable) return false;
            return !HasQuantity(itemId, quantity);
        }

        public bool CanTrade(string itemId)
        {
            var item = ItemRepo.GetItem(itemId);
            if (item == null || item.Untradeable) return false;
            return GetInstanceByItemId(itemId) != null;
        }

        public bool CanTradeInstance(string itemInstanceId)
        {
            var instance = GetItemInstance(itemInstanceId);
            if (instance == null) return false;
            var item = ItemRepo.GetItem(instance.ItemId);
            return item != null && !item.Untradeable;
        }

        public bool HasQuantity(string itemId, int quantity)
        {
            return quantity <= QuantityOf(itemId);
        }

        public SyncDictionary<string, ItemInstance> GetVisibleItems()
        {
            if (PublicItems.Count > 0 && PrivateItems.Count == 0)
                return PublicItems;
            return PrivateItems;
        }
        public int QuantityOf(string itemId)
        {
            return GetVisibleItems().Aggregate(0, (acc, element) =>
            {
                if (element.Value.ItemId == itemId) return acc + element.Value.Quantity;
                return acc;
            });
        }

        public bool RemoveItemInstance(string itemInstanceId, int quantity, bool allowPullingFromOthers = false)
        {
            if (!IsServer) return false;
            var instance = GetItemInstance(itemInstanceId);
            if (instance == null)
                return false;
            if (!allowPullingFromOthers && instance.Quantity < quantity)
                return false;
            if (allowPullingFromOthers && !HasQuantity(instance.ItemId, quantity))
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
            var instance = GetInstanceByItemId(itemId);
            if (!validated && !HasQuantity(instance.ItemId, quantity)) return false;
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

        public ItemInstance SplitStack(string itemInstanceId, int quantity)
        {
            var instance = GetItemInstance(itemInstanceId);
            if (instance != null && RemoveItemInstance(instance.ItemInstanceId, quantity))
                return AddItem(instance.ItemId, quantity, true);
            return null;
        }

        public void UpdateItemInstance(ItemInstance instance, bool isPublic = false)
        {
            if (!IsServer) return;
            PrivateItems[instance.ItemInstanceId] = new ItemInstance
            {
                OwnerId = OwnerEntity.entityId,
                ItemId = instance.ItemId,
                ItemInstanceId = instance.ItemInstanceId,
                Quantity = instance.Quantity,
                StorageId = StorageId,
                IsEquipped = instance.IsEquipped
            };
            if (isPublicStorage || instance.IsEquipped || isPublic)
                PublicItems[instance.ItemInstanceId] = PrivateItems[instance.ItemInstanceId];
            else if (PublicItems.ContainsKey(instance.ItemInstanceId))
                PublicItems.Remove(instance.ItemInstanceId);
            ItemManager.Instance.ItemsByInstanceId[instance.ItemInstanceId] = PrivateItems[instance.ItemInstanceId];
            if (instance.Quantity != 0) return;
            if (PublicItems.ContainsKey(instance.ItemInstanceId))
                PublicItems.Remove(instance.ItemInstanceId);
            PrivateItems.Remove(instance.ItemInstanceId);
            ItemManager.Instance.ItemsByInstanceId.Remove(instance.ItemInstanceId);
        }
    }
}