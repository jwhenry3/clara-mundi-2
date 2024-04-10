using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ClaraMundi
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance;
        public static event Action<SyncDictionaryOperation, int, ItemInstance, bool> ItemChange;
        [ShowInInspector]
        public readonly Dictionary<int, ItemInstance> ItemsByInstanceId = new();
        public readonly Dictionary<string, Dictionary<string, ItemStorage>> StorageByEntityAndId = new();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterStorage(ItemStorage storage)
        {
            if (!StorageByEntityAndId.ContainsKey(storage.OwnerEntity.entityId.Value))
            {
                StorageByEntityAndId[storage.OwnerEntity.entityId.Value] = new();
            }

            if (StorageByEntityAndId[storage.OwnerEntity.entityId.Value].ContainsKey(storage.StorageId.Value)) return;
            StorageByEntityAndId[storage.OwnerEntity.entityId.Value][storage.StorageId.Value] = storage;
            storage.Items.OnChange += OnChange;
            foreach (var kvp in storage.Items)
                ItemsByInstanceId[kvp.Key] = kvp.Value;
        }

        public void RemoveStorage(ItemStorage storage)
        {
            if (!(bool)storage.OwnerEntity || !StorageByEntityAndId.ContainsKey(storage.OwnerEntity.entityId.Value)) return;
            StorageByEntityAndId[storage.OwnerEntity.entityId.Value].Remove(storage.StorageId.Value);
            if (StorageByEntityAndId[storage.OwnerEntity.entityId.Value].Count == 0)
                StorageByEntityAndId.Remove(storage.OwnerEntity.entityId.Value);
            storage.Items.OnChange -= OnChange;
        }
        
        public void OnChange(SyncDictionaryOperation op, int key, ItemInstance itemInstance, bool asServer)
        {
            if (op == SyncDictionaryOperation.Add)
            {
                ItemsByInstanceId[key] = itemInstance;
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