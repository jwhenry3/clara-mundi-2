using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance;
        public static event Action<SyncDictionaryOperation, int, ItemInstance, bool> ItemChange;

        public readonly Dictionary<int, ItemInstance> ItemsByInstanceId = new();
        public readonly Dictionary<string, Dictionary<string, ItemStorage>> StorageByEntityAndId = new();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterStorage(ItemStorage storage)
        {
            if (!StorageByEntityAndId.ContainsKey(storage.OwnerEntity.entityId))
            {
                StorageByEntityAndId[storage.OwnerEntity.entityId] = new();
            }

            if (StorageByEntityAndId[storage.OwnerEntity.entityId].ContainsKey(storage.StorageId)) return;
            StorageByEntityAndId[storage.OwnerEntity.entityId][storage.StorageId] = storage;
            storage.PublicItems.OnChange += OnChange;
            storage.PrivateItems.OnChange += OnChange;
        }

        public void RemoveStorage(ItemStorage storage)
        {
            if (!(bool)storage.OwnerEntity || !StorageByEntityAndId.ContainsKey(storage.OwnerEntity.entityId)) return;
            StorageByEntityAndId[storage.OwnerEntity.entityId].Remove(storage.StorageId);
            if (StorageByEntityAndId[storage.OwnerEntity.entityId].Count == 0)
                StorageByEntityAndId.Remove(storage.OwnerEntity.entityId);
            storage.PublicItems.OnChange -= OnChange;
            storage.PrivateItems.OnChange -= OnChange;
        }
        
        public void OnChange(SyncDictionaryOperation op, int key, ItemInstance itemInstance, bool asServer)
        {
            if (op == SyncDictionaryOperation.Add)
            {
                if (!ItemsByInstanceId.ContainsKey(key))
                    ItemsByInstanceId.Add(key, itemInstance);
                ItemChange?.Invoke(op, key, itemInstance, asServer);
            }
            if (op == SyncDictionaryOperation.Remove)
            {
                if (ItemsByInstanceId.ContainsKey(key))
                    ItemsByInstanceId.Remove(key);
                ItemChange?.Invoke(op, key, itemInstance, asServer);
            }
            if (op == SyncDictionaryOperation.Set)
            {
                ItemsByInstanceId[key] = itemInstance;
                ItemChange?.Invoke(op, key, itemInstance, asServer);
            }

        }

        public ItemStorage GetStorageForItemInstance(ItemInstance instance)
        {
            if (!StorageByEntityAndId.ContainsKey(instance.CharacterId)) return null;
            if (StorageByEntityAndId[instance.CharacterId].ContainsKey(instance.StorageId))
                return StorageByEntityAndId[instance.CharacterId][instance.StorageId];
            return null;
        }
        public ItemInstance GetItemByInstanceId(int itemInstanceId)
        {
            return ItemsByInstanceId.ContainsKey(itemInstanceId) ? ItemsByInstanceId[itemInstanceId] : null;
        }
    }
}