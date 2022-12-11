using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
    public class EquipmentController : PlayerController
    {
        [SyncObject] public readonly SyncDictionary<string, string> EquippedItems = new();

        public bool ServerEquip(string itemInstanceId, bool reportErrors = false)
        {
            if (!IsServer) return false;
            ItemInstance instance = ItemManager.Instance.GetItemByInstanceId(itemInstanceId);
            if (instance == null) return false;
            if (instance.OwnerId != player.entityId) return false;
            return ServerEquipInstance(instance);
        }

        private bool ServerEquipInstance(ItemInstance instance)
        {
            if (!IsServer) return false;
            if (instance.OwnerId != player.entityId) return false;
            Item item = RepoManager.Instance.ItemRepo.GetItem(instance.ItemId);
            if (item == null || !item.Equippable) return false;
            if (EquippedItems.ContainsKey(item.EquipmentSlot))
            {
                if (EquippedItems[item.EquipmentSlot] == instance.ItemInstanceId) return false;
                ServerUnequip(EquippedItems[item.EquipmentSlot]);
            }

            var storage = ItemManager.Instance.GetStorageForItemInstance(instance);

            EquippedItems[item.EquipmentSlot] = instance.ItemInstanceId;
            instance.IsEquipped = true;
            if (storage != null)
                storage.UpdateItemInstance(instance);

            return true;
        }

        private bool ServerUnequipInstance(ItemInstance instance)
        {
            if (!IsServer) return false;
            if (instance.OwnerId != player.entityId) return false;
            Item item = RepoManager.Instance.ItemRepo.GetItem(instance.ItemId);
            if (item == null || !item.Equippable) return false;
            if (!EquippedItems.ContainsKey(item.EquipmentSlot)) return false;
            if (EquippedItems[item.EquipmentSlot] != instance.ItemInstanceId) return false;
            var equippedItemStorage = ItemManager.Instance.GetStorageForItemInstance(instance);
            instance.IsEquipped = false;
            if (equippedItemStorage != null)
                equippedItemStorage.UpdateItemInstance(instance);
            EquippedItems[item.EquipmentSlot] = null;

            return true;
        }

        public bool ServerUnequip(string itemInstanceId)
        {
            if (!IsServer) return false;
            if (string.IsNullOrEmpty(itemInstanceId)) return false;
            ItemInstance instance = ItemManager.Instance.GetItemByInstanceId(itemInstanceId);
            if (instance == null) return false;
            if (instance.OwnerId != player.entityId) return false;
            ServerUnequipInstance(instance);
            return true;
        }
    }
}