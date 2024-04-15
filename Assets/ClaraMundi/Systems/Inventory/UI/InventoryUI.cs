using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class InventoryUI : PlayerUI, IPointerDownHandler
  {
    readonly OwningEntityHolder owner = new();
    public ItemUI ItemNodePrefab;
    [HideInInspector]
    public ItemUI ContextualItem;

    public Transform Equipment;
    public Transform Consumables;
    public Transform General;
    public Transform QuestItems;

    public Form Form;


    private void Awake()
    {
      Reload();
    }

    private void OnEnable()
    {
      Reload();
    }

    public void Reload()
    {
      CleanUp();
      Populate();
    }

    private void CleanUp()
    {
      foreach (Transform child in Equipment.transform)
        Destroy(child.gameObject);
      foreach (Transform child in Consumables.transform)
        Destroy(child.gameObject);
      foreach (Transform child in General.transform)
        Destroy(child.gameObject);
      foreach (Transform child in QuestItems.transform)
        Destroy(child.gameObject);
    }

    private void Populate()
    {
      if (player == null) return;
      if (player.Inventory == null) return;
      if (player.Inventory.ItemStorage == null) return;
      foreach (var kvp in player.Inventory.ItemStorage.Items)
      {
        var itemInstance = kvp.Value;
        var item = RepoManager.Instance.ItemRepo.GetItem(itemInstance.ItemId);
        var parent = General;
        switch (item.Type)
        {
          case ItemType.Armor:
          case ItemType.Weapon:
            parent = Equipment;
            break;
          case ItemType.Consumable:
            parent = Consumables;
            break;
          case ItemType.KeyItem:
            parent = QuestItems;
            break;
          case ItemType.Ingredient:
          case ItemType.Generic:
          default:
            parent = General;
            break;
        }
        var instance = Instantiate(ItemNodePrefab, parent, false);
        instance.ShowEquippedStatus = true;
        instance.ItemInstanceId = itemInstance.ItemInstanceId;
        instance.SetOwner(owner);
        instance.OnDoubleClick += OnUseOrEquipItem;
      }
      Form?.InitializeElements();
    }
    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if (entity == null) return;
      owner.SetEntity(entity);
      Reload();
    }

    public void OnUseOrEquipItem(ItemUI item)
    {
      switch (item.Item.Type)
      {
        case ItemType.Armor:
        case ItemType.Weapon:
          player.Inventory.EquipItem(item.ItemInstance.ItemInstanceId, true);
          break;
        case ItemType.Consumable:
          player.Inventory.UseItem(item.ItemInstance.ItemInstanceId, 1);
          break;
        default:
          return;
      }
      Form?.PreviouslySelected?.Activate();
    }

    public void CloseContextMenu()
    {
      ContextMenuHandler.Instance.ItemMenu.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (ContextMenuHandler.Instance.ContextualItem != null)
      {
        CloseContextMenu();
      }
      else
      {
        if (Form?.PreviouslySelected != null)
          Form.PreviouslySelected.Activate();
      }
    }

    public void DropItem()
    {
      player.Inventory.DropItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, 1);
      CloseContextMenu();
    }
    public void EquipItem()
    {
      player.Inventory.EquipItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, true);
      CloseContextMenu();
    }
    public void UseItem()
    {
      player.Inventory.UseItem(ContextMenuHandler.Instance.ContextualItem.ItemInstance.ItemInstanceId, 1);
      CloseContextMenu();
    }
    public void LinkToChat()
    {
      ContextMenuHandler.Instance.ContextualItem.LinkToChat();
      CloseContextMenu();
    }

  }
}
