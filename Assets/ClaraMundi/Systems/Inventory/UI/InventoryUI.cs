using System;
using System.Collections;
using TMPro;
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
    public Dialog AmountDialog;
    public TMP_InputField AmountInput;
    public ItemTooltipUI Tooltip;

    [HideInInspector]
    public ItemUI ContextualItem;

    public Transform Equipment;
    public Transform Consumables;
    public Transform General;
    public Transform QuestItems;

    private ItemInstance DroppingItem;




    private void Awake()
    {
      Instance = this;
      Focus ??= GetComponent<CanvasGroupFocus>();
      CleanUp();
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
        instance.InventoryUI = this;
        instance.ShowEquippedStatus = true;
        instance.Tooltip = Tooltip;
        instance.ContextMenu = ItemContextMenu;
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
      ItemContextMenu.ContextualItem = null;
      ItemContextMenu.gameObject.SetActive(false);
      Focus.Select();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      CloseContextMenu();
    }

    public void DropItem(ItemUI item)
    {
      if (item.Item.Stackable || item.ItemInstance.Quantity > 1)
      {
        DroppingItem = item.ItemInstance;
        AmountDialog.Context = "Drop Item";
        AmountInput.text = item.ItemInstance.Quantity + "";
        AmountDialog.gameObject.SetActive(true);
      }
      else
      {
        player.Inventory.DropItem(item.ItemInstance.ItemInstanceId, 1);
        CloseContextMenu();
      }
    }
    public void EquipItem(ItemUI item)
    {
      player.Inventory.EquipItem(item.ItemInstance.ItemInstanceId, true);
      CloseContextMenu();
    }
    public void UnequipItem(ItemUI item)
    {
      player.Inventory.UnequipItem(item.ItemInstance.ItemInstanceId);
      CloseContextMenu();
    }
    public void UseItem(ItemUI item)
    {
      player.Inventory.UseItem(item.ItemInstance.ItemInstanceId, 1);
      CloseContextMenu();
    }
    public void LinkToChat(ItemUI item)
    {
      ChatWindowUI.Instance.AddItemLink(item.ItemInstance);
      CloseContextMenu();
    }

    public void OnDialogConfirm(Dialog dialog)
    {
      if (dialog.Context == "Drop Item")
      {
        CloseContextMenu();
        if (Int32.TryParse(AmountInput.text, out int amount))
          player.Inventory.DropItem(DroppingItem.ItemInstanceId, amount);
      }
      dialog.gameObject.SetActive(false);
      dialog.Context = "";
    }
    public void OnDialogCancel(Dialog dialog)
    {
      if (dialog.Context == "Drop Item")
      {
        CloseContextMenu();
      }
      dialog.gameObject.SetActive(false);
      dialog.Context = "";
    }
    public void OnDialogClose(Dialog dialog)
    {
      if (dialog.Context == "Drop Item")
      {
        CloseContextMenu();
      }
      dialog.gameObject.SetActive(false);
      dialog.Context = "";
    }

  }
}
