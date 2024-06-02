using UnityEngine;
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
    void OnEnable()
    {
      player.Actions.OnAction += OnAction;
    }

    void OnDisable()
    {

    }

    void OnAction(ActionInvocation action)
    {
      if (action.Action.Command == "/item")
      {
        if (!action.Args.ContainsKey("item"))
        {
          player.Chat.Channel.ServerSendMessage(new()
          {
            Channel = "System",
            Type = ChatMessageType.Error,
            Message = "Invalid item."
          });
          return;
        }
        var itemIdOrName = action.Args["item"];
        var item = ItemStorage.ItemRepo.GetItem(itemIdOrName);
        if (item == null)
        {
          player.Chat.Channel.ServerSendMessage(new()
          {
            Channel = "System",
            Type = ChatMessageType.Error,
            Message = "Invalid item."
          });
          return;
        }
        var instance = ItemStorage.GetInstanceByItemId(item.ItemId, true);
        if (instance == null)
        {
          player.Chat.Channel.ServerSendMessage(new()
          {
            Channel = "System",
            Type = ChatMessageType.Error,
            Message = "Invalid item."
          });
          return;
        }
        ServerUseItem(instance.ItemInstanceId, 1);
      }
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
    public void UseItem(int itemInstanceId, int quantity = 1)
    {
      ServerUseItem(itemInstanceId, quantity);
    }

    [ServerRpc]
    public void DropItem(int itemInstanceId, int quantity = 1)
    {
      ServerDropItem(itemInstanceId, quantity);
    }

    [ServerRpc]
    public void SplitStack(int itemInstanceId, int quantity = 1)
    {
      ServerSplitStack(itemInstanceId, quantity);
    }

    public bool ServerUseItem(int itemInstanceId, int quantity)
    {
      if (!IsServerStarted) return false;
      // use item should have a cast time and then apply a list of effects or do something
      // preferably use the action proxy to asynchronously perform the updates
      var instance = ItemStorage.GetItemInstance(itemInstanceId, true);
      if (instance == null) return false;
      var item = ItemRepo.GetItem(instance.ItemId);
      if (item == null) return false;
      player.Chat.Channel.ServerSendMessage(new()
      {
        Channel = "System",
        Message = "Used " + item.Name
      });
      // TODO: send action events based on item impacts
      return ItemStorage.RemoveItemInstance(itemInstanceId, quantity, true);
    }

    public bool ServerDropItem(int itemInstanceId, int quantity)
    {
      if (!IsServerStarted) return false;
      // Debug.Log(itemInstanceId);
      if (!ItemManager.Instance.ItemsByInstanceId.ContainsKey(itemInstanceId)) return false;
      // Debug.Log("Item Exists");
      var instance = ItemManager.Instance.ItemsByInstanceId[itemInstanceId];
      // Debug.Log(instance.ItemId + " - " + quantity);
      var canDrop = ItemStorage.CanDrop(instance.ItemId, quantity);
      var removed = canDrop && ItemStorage.RemoveItemInstance(itemInstanceId, quantity, true);
      // Debug.Log(canDrop + " - " + removed);
      return removed;
    }

    public ItemInstance ServerObtainItem(string itemId, int quantity = 1)
    {
      if (!IsServerStarted) return null;
      // Operation can only be performed on the server through an authorized action
      // via opening chest, looting an enemy, winning an auction, transfering from storage, etc
      return ItemStorage.AddItem(itemId, quantity);
    }

    public bool ServerSplitStack(int itemInstanceId, int quantity)
    {
      if (!IsServerStarted) return false;
      return ItemStorage.SplitStack(itemInstanceId, quantity) != null;
    }

    public void GetItemAndInstance(int itemInstanceId, out Item item, out ItemInstance itemInstance)
    {
      itemInstance = GetItem(itemInstanceId);
      item = itemInstance != null ? ItemRepo.GetItem(itemInstance.ItemId) : null;
    }
  }
}