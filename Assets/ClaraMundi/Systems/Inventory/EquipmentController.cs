using System.Collections.Generic;
using FishNet.Object.Synchronizing;

namespace ClaraMundi
{
  public class EquipmentController : PlayerController
  {

    public readonly SyncVar<EquipmentSet> CurrentSet = new();

    public override void OnStartServer()
    {
      base.OnStartServer();
      if (CurrentSet.Value == null)
        CurrentSet.Value = new();
    }

    public bool ServerEquip(int itemInstanceId)
    {
      if (!IsServerStarted) return false;
      ItemInstance instance = ItemManager.Instance.GetItemByInstanceId(itemInstanceId);
      if (instance == null) return false;
      if (instance.CharacterId != player.entityId) return false;
      return ServerEquipInstance(instance);
    }

    private Item GetEquippableItem(string itemId)
    {
      Item item = RepoManager.Instance.ItemRepo.GetItem(itemId);
      if (item != null && item.Equippable) return item;
      return null;
    }

    public ItemInstance GetEquippedItemInstance(string slot)
    {
      if (CurrentSet.Value == null) return null;
      var currentlyEquipped = CurrentSet.Value.Get(slot);
      if (currentlyEquipped == -1) return null;
      return ItemManager.Instance.GetItemByInstanceId(currentlyEquipped);
    }

    private bool ServerEquipInstance(ItemInstance instance)
    {
      if (!IsServerStarted) return false;
      if (instance.CharacterId != player.entityId) return false;
      Item item = GetEquippableItem(instance.ItemId);
      if (item == null) return false;
      var currentlyEquipped = CurrentSet.Value.Get(item.EquipmentSlot);
      if (currentlyEquipped == instance.ItemInstanceId)
        return false;
      if (currentlyEquipped > -1)
        ServerUnequipInstance(ItemManager.Instance.GetItemByInstanceId(currentlyEquipped));

      UpdateStorage(instance, true);
      SetSlotValue(item.EquipmentSlot, instance.ItemInstanceId);
      UpdateStats(item, true);
      player.Chat.Channel.ServerSendMessage(new()
      {
        Channel = "System",
        Message = "Equipped " + item.Name
      });
      return true;
    }

    private void UpdateStats(Item item, bool equipped)
    {
      player.Stats.UpdateStatModifications(item.StatModifications, equipped);
      player.Stats.UpdateAttributeModifications(item.AttributeModifications, equipped);
      player.Stats.ComputeStats();
    }
    private void SetSlotValue(string slot, int value)
    {
      var clone = CurrentSet.Value.Clone();
      clone.Set(slot, value);
      CurrentSet.Value = clone;
    }

    private void UpdateStorage(ItemInstance instance, bool equipped)
    {
      instance.IsEquipped = equipped;
      var storage = ItemManager.Instance.GetStorageForItemInstance(instance);
      if (storage != null)
        storage.UpdateItemInstance(instance);
    }

    private bool ServerUnequipInstance(ItemInstance instance)
    {
      if (!IsServerStarted) return false;
      if (instance.CharacterId != player.entityId) return false;
      Item item = GetEquippableItem(instance.ItemId);
      if (item == null) return false;
      var currentlyEquipped = CurrentSet.Value.Get(item.EquipmentSlot);
      if (currentlyEquipped != instance.ItemInstanceId) return false;

      UpdateStorage(instance, false);
      SetSlotValue(item.EquipmentSlot, -1);
      UpdateStats(item, false);

      player.Chat.Channel.ServerSendMessage(new()
      {
        Channel = "System",
        Message = "Unequipped " + item.Name
      });
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
      if (CurrentSet.Value == null)
        CurrentSet.Value = new();
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Main));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Sub));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ranged));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ammo));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Head));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Neck));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Body));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Hands));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Back));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Waist));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Legs));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Feet));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ear1));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ear2));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ring1));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(CurrentSet.Value.Ring2));
    }

    public void ServerEquipAll(EquipmentSet equipment)
    {
      if (!IsServerStarted) return;
      CurrentSet.Value = new();
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Main));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Sub));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ranged));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ammo));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Head));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Neck));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Body));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Hands));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Back));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Waist));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Legs));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Feet));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ear1));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ear2));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ring1));
      ServerEquipInstance(player.Inventory.ItemStorage.GetItemInstance(equipment.Ring2));
    }
  }
}