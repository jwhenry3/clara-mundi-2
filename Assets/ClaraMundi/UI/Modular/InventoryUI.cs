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
    public bool forLocalPlayer = true;
    public string playerName;

    private InventoryController inventory;

    private Dictionary<int, InventoryItemUI> items = new();
    void OnEnable()
    {
      inventory = null;
      if (PlayerManager.Instance == null) return;
      var player = PlayerManager.Instance.LocalPlayer;
      if (window == null)
        window = GetComponentInParent<WindowUI>();
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
          items.Remove(key);
          Destroy(items[key].gameObject);
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
        if (index == 0)
        {
          EventSystem.current.SetSelectedGameObject(itemUI.gameObject);
        }
        index++;
      }
    }
  }
}