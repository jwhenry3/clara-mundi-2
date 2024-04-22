using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class InventoryUI : PlayerUI, IPointerDownHandler
  {
    public static InventoryUI Instance;
    readonly OwningEntityHolder owner = new();
    public ItemUI ItemNodePrefab;

    public CanvasGroupFocus Focus;
    public ContextMenu ItemContextMenu;

    [HideInInspector]
    public ItemUI ContextualItem;

    public Transform Equipment;
    public Transform Consumables;
    public Transform General;
    public Transform QuestItems;



    private void Awake()
    {
      Instance = this;
      Focus ??= GetComponent<CanvasGroupFocus>();
    }

    private void OnEnable()
    {
      if (ItemManager.Instance == null) return;
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
        instance.gameObject.name = item.Name;
        instance.OnDoubleClick += OnUseOrEquipItem;
      }
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
      Focus.Select();
    }

    public void CloseContextMenu()
    {
      EventSystem.current.SetSelectedGameObject(ItemContextMenu.ContextualItem?.gameObject);
      ItemContextMenu.ContextualItem = null;
      ItemContextMenu.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (ItemContextMenu.ContextualItem != null)
        CloseContextMenu();
      else
        Focus.Select();
    }

    public void DropItem()
    {
      player.Inventory.DropItem(ItemContextMenu.ContextualItem.ItemInstance.ItemInstanceId, 1);
      CloseContextMenu();
    }
    public void EquipItem()
    {
      player.Inventory.EquipItem(ItemContextMenu.ContextualItem.ItemInstance.ItemInstanceId, true);
      CloseContextMenu();
    }
    public void UnequipItem()
    {
      player.Inventory.UnequipItem(ItemContextMenu.ContextualItem.ItemInstance.ItemInstanceId);
      CloseContextMenu();
    }
    public void UseItem()
    {
      player.Inventory.UseItem(ItemContextMenu.ContextualItem.ItemInstance.ItemInstanceId, 1);
      CloseContextMenu();
    }
    public void LinkToChat()
    {
      ItemContextMenu.ContextualItem.LinkToChat();
      CloseContextMenu();
    }

  }
}
