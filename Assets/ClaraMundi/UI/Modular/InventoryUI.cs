using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
  public class InventoryUI : MonoBehaviour
  {
    public WindowUI window;
    public CanvasGroupWatcher watcher;
    public InventoryItemUI ItemPrefab;
    public GameObject NoItemPrefab;
    public Transform ItemsContainer;
    public WindowUI ItemMenu;
    public CanvasGroupWatcher ItemMenuWatcher;

    public ItemTooltipUI tooltip;

    public GameObject UseMenuItem;
    public GameObject EquipMenuItem;
    public GameObject UnequipMenuItem;
    public GameObject DropMenuItem;
    public bool forLocalPlayer = true;
    public string playerName;

    private InventoryController inventory;

    private InventoryItemUI chosenItem;

    private Player player;

    private Dictionary<int, InventoryItemUI> items = new();
    void OnEnable()
    {
      inventory = null;
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
      if (!forLocalPlayer)
      {
        if (!PlayerManager.Instance.PlayersByName.ContainsKey(playerName))
        {
          return;
        }
        player = PlayerManager.Instance.PlayersByName[playerName];
      }
      if (player == null) return;

      inventory = player.Inventory;
      inventory.ItemStorage.Items.OnChange += OnChange;
      LoadItems();
    }
    void OnDisable()
    {
      if (inventory != null)
      {
        inventory.ItemStorage.Items.OnChange -= OnChange;
      }
    }

    private void OnChange(SyncDictionaryOperation op, int key, ItemInstance value, bool asServer)
    {
      if (op == SyncDictionaryOperation.Add)
        AddItem(key, value);
      if (op == SyncDictionaryOperation.Set)
        UpdateItem(key, value);
      if (op == SyncDictionaryOperation.Remove)
        RemoveItem(key);
    }
    void AddItem(int itemInstanceId, ItemInstance instance)
    {
      if (items.ContainsKey(itemInstanceId)) return;
      var item = inventory.ItemRepo.GetItem(instance.ItemId);
      var itemUI = Instantiate(ItemPrefab, ItemsContainer);
      itemUI.instance = instance;
      itemUI.item = item;
      itemUI.SetUp();
      items[itemInstanceId] = itemUI;
    }
    void UpdateItem(int itemInstanceId, ItemInstance instance)
    {
      var item = inventory.ItemRepo.GetItem(instance.ItemId);
      if (!items.ContainsKey(itemInstanceId))
        items[itemInstanceId] = Instantiate(ItemPrefab, ItemsContainer);
      items[itemInstanceId].instance = instance;
      items[itemInstanceId].item = item;
      items[itemInstanceId].SetUp();
    }
    void RemoveItem(int itemInstanceId)
    {
      if (items.ContainsKey(itemInstanceId))
      {
        items[itemInstanceId].OnChosen -= OnChosen;
        if (EventSystem.current.currentSelectedGameObject == items[itemInstanceId].gameObject)
        {
          var obj = EventSystem.current.currentSelectedGameObject;
          var container = obj.transform.parent;
          if (container.childCount > 1)
          {
            var index = obj.transform.GetSiblingIndex();
            var change = index > 0 ? -1 : 1;
            EventSystem.current.SetSelectedGameObject(container.GetChild(index + change).gameObject);
          }
        }
        Destroy(items[itemInstanceId].gameObject);
        items.Remove(itemInstanceId);
      }
    }

    void LoadItems()
    {
      watcher.CurrentButton = null;
      watcher.AutoFocusButton = null;
      items = new();
      foreach (Transform child in ItemsContainer)
        Destroy(child.gameObject);
      int index = 0;
      foreach (var kvp in inventory.ItemStorage.Items)
      {
        var item = inventory.ItemRepo.GetItem(kvp.Value.ItemId);
        var itemUI = Instantiate(ItemPrefab, ItemsContainer);
        itemUI.instance = kvp.Value;
        itemUI.item = item;
        itemUI.SetUp();
        items[kvp.Key] = itemUI;
        itemUI.OnChosen += OnChosen;
        if (index == 0)
        {
          EventSystem.current.SetSelectedGameObject(itemUI.gameObject);
        }
        index++;
      }
      if (index == 0)
      {
        Instantiate(NoItemPrefab, ItemsContainer);
      }
    }
    void OnChosen(InventoryItemUI itemUI)
    {
      chosenItem = itemUI;
      ItemMenuWatcher.CurrentButton = null;
      if (chosenItem.item.Usable)
        UseMenuItem.GetComponent<ButtonUI>().AutoFocus = true;
      else if (chosenItem.item.Equippable && !chosenItem.instance.IsEquipped)
        EquipMenuItem.GetComponent<ButtonUI>().AutoFocus = true;
      else
      if (chosenItem.item.Equippable && chosenItem.instance.IsEquipped)
        UnequipMenuItem.GetComponent<ButtonUI>().AutoFocus = true;
      else
      if (chosenItem.item.Droppable)
        DropMenuItem.GetComponent<ButtonUI>().AutoFocus = true;
      UseMenuItem?.SetActive(chosenItem.item.Usable);
      EquipMenuItem?.SetActive(chosenItem.item.Equippable && !chosenItem.instance.IsEquipped);
      UnequipMenuItem?.SetActive(chosenItem.item.Equippable && chosenItem.instance.IsEquipped);
      DropMenuItem?.SetActive(chosenItem.item.Droppable);
      ItemMenu.moveSibling.ToFront();
    }

    void Update()
    {
      if (!ItemMenu.moveSibling.IsInFront())
        chosenItem = null;
      InventoryItemUI selectedItem = EventSystem.current.currentSelectedGameObject?.GetComponent<InventoryItemUI>();
      if (selectedItem != null)
      {
        tooltip.SetItemInstance(selectedItem.instance);
      }
      if (tooltip.gameObject.activeInHierarchy != (selectedItem != null))
        tooltip.gameObject.SetActive(selectedItem != null);
    }

    public void Use()
    {
      ItemMenu.moveSibling.ToBack();
      if (chosenItem == null) return;
      if (!chosenItem.item.Usable) return;
      player.Inventory.UseItem(chosenItem.instance.ItemInstanceId, 1);
    }
    public void Equip()
    {
      ItemMenu.moveSibling.ToBack();
      if (chosenItem == null) return;
      if (chosenItem.item.Equippable && !chosenItem.instance.IsEquipped)
      {
        player.Inventory.EquipItem(chosenItem.instance.ItemInstanceId);
      }

    }
    public void Unequip()
    {
      ItemMenu.moveSibling.ToBack();
      if (chosenItem == null) return;
      if (chosenItem.item.Equippable && chosenItem.instance.IsEquipped)
      {
        player.Inventory.UnequipItem(chosenItem.instance.ItemInstanceId);
      }

    }
    public void Assign()
    {
      ItemMenu.moveSibling.ToBack();
      if (chosenItem == null) return;
      ActionBarUI.Instance.CurrentActionBarAction = chosenItem.action;
      ActionBarUI.Instance.SelectFirstAction1();
    }

    public void Drop()
    {
      ItemMenu.moveSibling.ToBack();
      if (chosenItem == null) return;
      if (!chosenItem.item.Droppable) return;
      player.Inventory.DropItem(chosenItem.instance.ItemInstanceId, 1);
    }
  }
}