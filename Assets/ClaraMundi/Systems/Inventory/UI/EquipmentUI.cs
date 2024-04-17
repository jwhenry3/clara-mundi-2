using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class EquipmentUI : PlayerUI, IPointerDownHandler
  {
    OwningEntityHolder owner = new();
    public ItemUI ItemNodePrefab;

    public Form Form;

    public Transform EquipmentContainer;
    public Transform RightHand;
    public Transform LeftHand;
    public Transform Head;
    public Transform Body;
    public Transform Hands;
    public Transform Legs;
    public Transform Feet;
    public Transform Back;
    public Transform Ring1;
    public Transform Ring2;
    public Transform Earring1;
    public Transform Earring2;
    public Transform Neck;

    public void OnEnable()
    {
      if (PlayerManager.Instance != null)
        OnPlayerChange(PlayerManager.Instance.LocalPlayer);
      Populate();
    }

    public void Clear()
    {
      foreach (ItemUI item in EquipmentContainer.GetComponentsInChildren<ItemUI>())
        Destroy(item.gameObject);
    }

    private string[] GetSlots()
    {
      return new string[] {
        "right_hand",
        "left_hand",
        "head",
        "body",
        "hands",
        "legs",
        "feet",
        "back",
        "ring1",
        "ring2",
        "earring1",
        "earring2",
        "neck",
      };
    }

    private Transform GetSlotContainer(string slotName)
    {
      return slotName switch
      {
        "right_hand" => RightHand,
        "left_hand" => LeftHand,
        "head" => Head,
        "body" => Body,
        "hands" => Hands,
        "legs" => Legs,
        "feet" => Feet,
        "back" => Back,
        "ring1" => Ring1,
        "ring2" => Ring2,
        "earring1" => Earring1,
        "earring2" => Earring2,
        "neck" => Neck,
        _ => null
      };
    }

    public void Populate()
    {
      if (player == null) return;
      foreach (var slot in GetSlots())
        LoadItem(GetSlotContainer(slot), slot);
    }

    private void LoadItem(Transform container, string equipmentSlot)
    {
      Debug.Log("Load Item: " + equipmentSlot);
      if (container == null) return;
      // Remove extra ItemUI instances
      ItemUI instance = container.GetComponentInChildren<ItemUI>();

      Debug.Log("Instance: " + instance?.name);
      if (instance == null)
      {
        instance = Instantiate(ItemNodePrefab, container, false);
        instance.transform.localPosition = Vector3.zero;
        instance.ShowEquippedStatus = false;
        instance.EquipmentUI = this;
        instance.EquipmentSlot = equipmentSlot;
        instance.GetComponent<FormElement>().Form = Form;
        Debug.Log("Initialized: " + equipmentSlot + " - " + instance.gameObject.name + " - ");
      }

      instance.SetOwner(owner);

    }

    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if (entity == null) return;
      owner.SetEntity(_player.Entity);
      Populate();
    }

    public void CloseContextMenu()
    {
      ContextMenuHandler.Instance.ContextualItem = null;
      ContextMenuHandler.Instance.ItemMenu.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      SelectFirstItem();
      if (ContextMenuHandler.Instance.ContextualItem != null)
        CloseContextMenu();
    }

    public void OnUnequip(ItemUI item)
    {
      Form.InitializeElements();
      player.Inventory.UnequipItem(item.ItemInstance.ItemInstanceId);
      CloseContextMenu();
    }

    public void SelectFirstItem()
    {
      var item = GetComponentInChildren<ItemUI>();
      if (item != null)
        EventSystem.current.SetSelectedGameObject(item.gameObject);
    }
  }
}