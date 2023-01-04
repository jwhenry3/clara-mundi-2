using FishNet.Object;

namespace ClaraMundi
{
    public class InventoryController : PlayerController
    {
        public ItemStorage ItemStorage;

        public EquipmentController Equipment => player.Equipment;

        public ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;

        public ItemInstance GetItem(int itemInstanceId)
        {
            return ItemStorage.GetItemInstance(itemInstanceId);
        }

        [ServerRpc]
        public void EquipItem(int itemInstanceId, bool tryUnequip = false)
        {
            if (!Equipment) return;
            if (!Equipment.ServerEquip(itemInstanceId, true) && tryUnequip)
                Equipment.ServerUnequip(itemInstanceId);
        }

        [ServerRpc]
        public void UnequipItem(int itemInstanceId)
        {
            if (!Equipment) return;
            Equipment.ServerUnequip(itemInstanceId);
        }

        [ServerRpc]
        public void UseItem(int itemInstanceId, int quantity)
        {
            ServerUseItem(itemInstanceId, quantity);
        }

        [ServerRpc]
        public void DropItem(int itemInstanceId, int quantity)
        {
            ServerDropItem(itemInstanceId, quantity);
        }

        [ServerRpc]
        public void SplitStack(int itemInstanceId, int quantity)
        {
            ServerSplitStack(itemInstanceId, quantity);
        }

        public bool ServerUseItem(int itemInstanceId, int quantity)
        {
            if (!IsServer) return false;
            // use item should have a cast time and then apply a list of effects or do something
            // preferably use the action proxy to asynchronously perform the updates
            // TODO: send action events based on item impacts
            return ItemStorage.RemoveItemInstance(itemInstanceId, quantity, true);
        }

        public bool ServerDropItem(int itemInstanceId, int quantity)
        {
            if (!IsServer) return false;
            if (!ItemManager.Instance.ItemsByInstanceId.ContainsKey(itemInstanceId)) return false;
            var instance = ItemManager.Instance.ItemsByInstanceId[itemInstanceId];
            return ItemStorage.CanDrop(instance.ItemId, quantity) &&
                   ItemStorage.RemoveItemInstance(itemInstanceId, quantity, true);
        }

        public ItemInstance ServerObtainItem(string itemId, int quantity = 1)
        {
            if (!IsServer) return null;
            // Operation can only be performed on the server through an authorized action
            // via opening chest, looting an enemy, winning an auction, transfering from storage, etc
            return ItemStorage.AddItem(itemId, quantity);
        }

        public bool ServerSplitStack(int itemInstanceId, int quantity)
        {
            if (!IsServer) return false;
            return ItemStorage.SplitStack(itemInstanceId, quantity) != null;
        }

        public void GetItemAndInstance(int itemInstanceId, out Item item, out ItemInstance itemInstance)
        {
            itemInstance = GetItem(itemInstanceId);
            item = itemInstance != null ? ItemRepo.GetItem(itemInstance.ItemId) : null;
        }
    }
}