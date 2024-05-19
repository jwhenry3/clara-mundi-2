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
    public InventoryItemUI ItemPrefab;
    public Transform ItemsContainer;
    public WindowUI ItemMenu;

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
      {
        var item = inventory.ItemRepo.GetItem(value.ItemId);
        var itemUI = Instantiate(ItemPrefab, ItemsContainer);
        itemUI.instance = value;
        itemUI.item = item;
        itemUI.SetUp();
        items[key] = itemUI;
      }
      if (op == SyncDictionaryOperation.Remove)
      {
        if (items.ContainsKey(key))
        {
          items[key].OnChosen -= OnChosen;
          if (EventSystem.current.currentSelectedGameObject == items[key].gameObject)
          {
            var obj = EventSystem.current.currentSelectedGameObject;
            var container = obj.transform.parent;
            if (container.childCount > 1)
            {
              var index = obj.transform.GetSiblingIndex();
              if (index > 0)
              {
                EventSystem.current.SetSelectedGameObject(container.GetChild(index - 1).gameObject);
              }
              else
              {
                EventSystem.current.SetSelectedGameObject(container.GetChild(index + 1).gameObject);
              }
            }
          }
          Destroy(items[key].gameObject);
          items.Remove(key);
        }
      }
      if (op == SyncDictionaryOperation.Set)
      {
        var item = inventory.ItemRepo.GetItem(value.ItemId);
        if (items.ContainsKey(key))
        {
          items[key].instance = value;
          items[key].item = item;
        }
        else
        {
          var itemUI = Instantiate(ItemPrefab, ItemsContainer);
          itemUI.instance = value;
          itemUI.item = item;
          itemUI.SetUp();
          items[key] = itemUI;
        }
      }
    }

    void LoadItems()
    {
      window.CurrentButton = null;
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
    }
    void OnChosen(InventoryItemUI itemUI)
    {
      chosenItem = itemUI;
      ItemMenu.CurrentButton = null;
      UseMenuItem?.SetActive(chosenItem.item.Usable);
      EquipMenuItem?.SetActive(chosenItem.item.Equippable && !chosenItem.instance.IsEquipped);
      UnequipMenuItem?.SetActive(chosenItem.item.Equippable && chosenItem.instance.IsEquipped);
      DropMenuItem?.SetActive(chosenItem.item.Droppable);
      if (chosenItem.item.Usable)
        ItemMenu.CurrentButton = UseMenuItem.GetComponent<ButtonUI>();
      else if (chosenItem.item.Equippable && !chosenItem.instance.IsEquipped)
        ItemMenu.CurrentButton = EquipMenuItem.GetComponent<ButtonUI>();
      else
      if (chosenItem.item.Equippable && chosenItem.instance.IsEquipped)
        ItemMenu.CurrentButton = UnequipMenuItem.GetComponent<ButtonUI>();
      else
      if (chosenItem.item.Droppable)
        ItemMenu.CurrentButton = DropMenuItem.GetComponent<ButtonUI>();
      ItemMenu.moveSibling.ToFront();
    }

    void Update()
    {
      if (!ItemMenu.moveSibling.IsInFront())
        chosenItem = null;
    }

    public void Use()
    {
      window.moveSibling.ToFront();
      EventSystem.current.SetSelectedGameObject(window.CurrentButton.gameObject);
      if (chosenItem == null) return;
      if (!chosenItem.item.Usable) return;
      player.Inventory.UseItem(chosenItem.instance.ItemInstanceId, 1);
    }
    public void Equip()
    {
      window.moveSibling.ToFront();
      EventSystem.current.SetSelectedGameObject(window.CurrentButton.gameObject);
      if (chosenItem == null) return;
      if (chosenItem.item.Equippable && !chosenItem.instance.IsEquipped)
      {
        player.Inventory.EquipItem(chosenItem.instance.ItemInstanceId);
      }

    }
    public void Unequip()
    {
      window.moveSibling.ToFront();
      EventSystem.current.SetSelectedGameObject(window.CurrentButton.gameObject);
      if (chosenItem == null) return;
      if (chosenItem.item.Equippable && chosenItem.instance.IsEquipped)
      {
        player.Inventory.UnequipItem(chosenItem.instance.ItemInstanceId);
      }

    }

    public void Drop()
    {
      window.moveSibling.ToFront();
      EventSystem.current.SetSelectedGameObject(window.CurrentButton.gameObject);
      if (chosenItem == null) return;
      if (!chosenItem.item.Droppable) return;
      player.Inventory.DropItem(chosenItem.instance.ItemInstanceId, 1);
    }
  }
}