using System.Collections.Generic;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class EquipmentController : PlayerController
    {
        public readonly SyncDictionary<string, int> EquippedItems = new(new SyncTypeSettings(ReadPermission.OwnerOnly));

        public readonly SyncDictionary<string, string> EquippedItemIds = new();

        public bool ServerEquip(int itemInstanceId, bool reportErrors = false)
        {
            if (!IsServerStarted) return false;
            ItemInstance instance = ItemManager.Instance.GetItemByInstanceId(itemInstanceId);
            if (instance == null) return false;
            if (instance.CharacterId != player.entityId) return false;
            return ServerEquipInstance(instance);
        }

        private bool ServerEquipInstance(ItemInstance instance)
        {
            if (!IsServerStarted) return false;
            if (instance.CharacterId != player.entityId) return false;
            Item item = RepoManager.Instance.ItemRepo.GetItem(instance.ItemId);
            if (item == null || !item.Equippable) return false;
            if (EquippedItems.ContainsKey(item.EquipmentSlot))
            {
                if (EquippedItems[item.EquipmentSlot] == instance.ItemInstanceId) return false;
                ServerUnequip(EquippedItems[item.EquipmentSlot]);
            }
            var storage = ItemManager.Instance.GetStorageForItemInstance(instance);
            EquippedItems[item.EquipmentSlot] = instance.ItemInstanceId;
            EquippedItemIds[item.EquipmentSlot] = instance.ItemId;
            instance.IsEquipped = true;
            if (storage != null)
                storage.UpdateItemInstance(instance);
            player.Stats.UpdateStatModifications(item.StatModifications, true);
            player.Stats.UpdateAttributeModifications(item.AttributeModifications, true);
            player.Stats.ComputeStats();
            return true;
        }

        private bool ServerUnequipInstance(ItemInstance instance)
        {
            if (!IsServerStarted) return false;
            if (instance.CharacterId != player.entityId) return false;
            Item item = RepoManager.Instance.ItemRepo.GetItem(instance.ItemId);
            if (item == null || !item.Equippable) return false;
            if (!EquippedItems.ContainsKey(item.EquipmentSlot)) return false;
            if (EquippedItems[item.EquipmentSlot] != instance.ItemInstanceId) return false;
            var equippedItemStorage = ItemManager.Instance.GetStorageForItemInstance(instance);
            instance.IsEquipped = false;
            if (equippedItemStorage != null)
                equippedItemStorage.UpdateItemInstance(instance);
            EquippedItems.Remove(item.EquipmentSlot);
            EquippedItemIds[item.EquipmentSlot] = null;

            player.Stats.UpdateStatModifications(item.StatModifications, false);
            player.Stats.UpdateAttributeModifications(item.AttributeModifications, false);
            player.Stats.ComputeStats();
            return true;
        }

        public bool ServerUnequip(int itemInstanceId)
        {
            if (!IsServerStarted) return false;
            ItemInstance instance = ItemManager.Instance.GetItemByInstanceId(itemInstanceId);
            if (instance == null) return false;
            if (instance.CharacterId != player.entityId) return false;
            ServerUnequipInstance(instance);
            return true;
        }

        public void ServerUnequipAll()
        {
            if (!IsServerStarted) return;
            foreach (var kvp in EquippedItems)
                ServerUnequipInstance(ItemManager.Instance.GetItemByInstanceId(kvp.Value));
        }

        public void ServerEquipAll(List<CharacterEquipment> equipment)
        {
            if (!IsServerStarted) return;
            foreach (CharacterEquipment eq in equipment)
                ServerEquipInstance(player.Inventory.ItemStorage.GetInstanceByItemId(eq.itemId));
        }
    }
}