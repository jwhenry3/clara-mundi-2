using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object.Synchronizing;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
  public class EquipmentUI : MonoBehaviour
  {
    public WindowUI window;
    public InventoryItemUI ItemPrefab;
    public GameObject NoItemPrefab;
    public Transform ItemsContainer;
    public CanvasGroup gridGroup;
    public CanvasGroupWatcher gridWatcher;
    public CanvasGroup itemsGroup;
    public CanvasGroupWatcher itemsWatcher;


    public ItemTooltipUI tooltip;


    public string slotFilter;
    public bool forLocalPlayer = true;
    public string playerName;

    public EquipmentItemUI CurrentSlot;

    private InventoryController inventory;
    private string chosenSlot;


    private Player player;

    private Dictionary<int, InventoryItemUI> items = new();

    void OnEnable()
    {
      slotFilter = "main";
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

      gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
      itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
      inventory = player.Inventory;
      inventory.ItemStorage.Items.OnChange += OnChange;
      LoadItems();
      window.CancelPressed += OnCancel;
    }

    void OnCancel()
    {
      chosenSlot = null;
      gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
      itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
    }
    void OnDisable()
    {
      chosenSlot = null;
      gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
      itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
      if (inventory != null)
        inventory.ItemStorage.Items.OnChange -= OnChange;
    }

    private void OnChange(SyncDictionaryOperation op, int key, ItemInstance value, bool asServer)
    {
      var item = value != null ? inventory.ItemRepo.GetItem(value.ItemId) : null;
      if (!string.IsNullOrEmpty(chosenSlot) && (item == null || item.EquipmentSlot != chosenSlot))
        RemoveItem(key);
      if (value != null && op == SyncDictionaryOperation.Add)
        AddItem(key, value);
      if (value != null && op == SyncDictionaryOperation.Set)
        UpdateItem(key, value);
      if (op == SyncDictionaryOperation.Remove)
        RemoveItem(key);
    }
    void AddItem(int itemInstanceId, ItemInstance instance)
    {
      if (items.ContainsKey(itemInstanceId)) return;
      var item = inventory.ItemRepo.GetItem(instance.ItemId);
      if (!item.Equippable) return;
      if (!string.IsNullOrEmpty(chosenSlot) && item.EquipmentSlot != chosenSlot) return;
      var itemUI = Instantiate(ItemPrefab, ItemsContainer);
      itemUI.instance = instance;
      itemUI.item = item;
      itemUI.SetUp();
      items[itemInstanceId] = itemUI;
    }
    void UpdateItem(int itemInstanceId, ItemInstance instance)
    {
      var item = inventory.ItemRepo.GetItem(instance.ItemId);
      if (!item.Equippable) return;
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
        items[itemInstanceId].OnChosen -= EquipOrUnequip;
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

    public void LoadItems()
    {
      itemsWatcher.CurrentButton = null;
      itemsWatcher.AutoFocusButton = null;
      items = new();
      foreach (Transform child in ItemsContainer)
        Destroy(child.gameObject);
      int index = 0;
      foreach (var kvp in inventory.ItemStorage.Items)
      {
        var item = inventory.ItemRepo.GetItem(kvp.Value.ItemId);
        if (!item.Equippable) continue;
        if (!string.IsNullOrEmpty(slotFilter) && item.EquipmentSlot != slotFilter) continue;

        var itemUI = Instantiate(ItemPrefab, ItemsContainer);
        itemUI.instance = kvp.Value;
        itemUI.item = item;
        itemUI.SetUp();
        items[kvp.Key] = itemUI;
        itemUI.OnChosen += EquipOrUnequip;
        index++;
      }
      if (index == 0)
      {
        Instantiate(NoItemPrefab, ItemsContainer);
      }
    }
    public void OnChosenSlot()
    {
      chosenSlot = null;
      gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
      itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
      if (CurrentSlot != null)
        EventSystem.current.SetSelectedGameObject(CurrentSlot.gameObject);
      CurrentSlot = null;
    }
    public void OnChosenSlot(EquipmentItemUI equipmentItemUI)
    {
      if (slotFilter != equipmentItemUI.equipmentSlot)
      {
        slotFilter = equipmentItemUI.equipmentSlot;
        LoadItems();
      }
      if (items.Count > 0)
      {
        var slot = equipmentItemUI.equipmentSlot;
        CurrentSlot = equipmentItemUI;
        chosenSlot = slot;
        gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
        itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
        EventSystem.current.SetSelectedGameObject(items.First().Value.gameObject);
      }
    }

    void Update()
    {
      InventoryItemUI selectedItem = EventSystem.current.currentSelectedGameObject?.GetComponent<InventoryItemUI>();
      EquipmentItemUI selectedEquipment = EventSystem.current.currentSelectedGameObject?.GetComponent<EquipmentItemUI>();
      bool displayTooltip = false;
      if (selectedItem != null)
      {
        tooltip.SetItemInstance(selectedItem.instance);
        displayTooltip = true;
      }
      if (selectedEquipment != null)
      {
        tooltip.SetItemInstance(selectedEquipment.instance);
        displayTooltip = selectedEquipment.instance != null;
      }
      if (tooltip.gameObject.activeInHierarchy != displayTooltip)
        tooltip.gameObject.SetActive(displayTooltip);

      window.blockCancel = !string.IsNullOrEmpty(chosenSlot);
      var lastGridEnabled = gridGroup.interactable;
      gridGroup.interactable = string.IsNullOrEmpty(chosenSlot);
      itemsGroup.interactable = !string.IsNullOrEmpty(chosenSlot);
      if (!lastGridEnabled && gridGroup.interactable && CurrentSlot != null)
      {
        EventSystem.current.SetSelectedGameObject(CurrentSlot.gameObject);
      }
    }

    void EquipOrUnequip(InventoryItemUI chosenItem)
    {
      OnChosenSlot();
      if (chosenItem == null) return;
      if (!chosenItem.item.Equippable) return;
      if (!chosenItem.instance.IsEquipped)
        player.Inventory.EquipItem(chosenItem.instance.ItemInstanceId);
      if (chosenItem.instance.IsEquipped)
        player.Inventory.UnequipItem(chosenItem.instance.ItemInstanceId);
    }
  }
}