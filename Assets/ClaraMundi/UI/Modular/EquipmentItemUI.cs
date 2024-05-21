using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ClaraMundi
{
  public class EquipmentItemUI : MonoBehaviour, ISelectHandler
  {
    EquipmentUI equipmentUI;
    ButtonUI button;
    public string equipmentSlot;
    private UnityEvent buttonClick;
    void OnEnable()
    {
      button = GetComponent<ButtonUI>();
      equipmentUI = GetComponentInParent<EquipmentUI>();
      if (Application.isPlaying)
      {
        if (button != null && button.button != null)
          (buttonClick = button.button.onClick).AddListener(OnClick);
      }
    }

    void OnClick()
    {
      equipmentSlot = gameObject.name.ToLower();
      equipmentUI.OnChosenSlot(this);
    }
    void OnDisable()
    {
      if (buttonClick != null)
      {
        buttonClick.RemoveListener(OnClick);
        buttonClick = null;
      }
    }

    void Update()
    {
      if (button == null) return;
      button.UseNameAsText = true;
      if (PlayerManager.Instance != null)
      {
        var player = PlayerManager.Instance.LocalPlayer;
        if (player == null) return;
        var equipment = player.Equipment.EquippedItemIds;
        equipmentSlot = gameObject.name.ToLower();
        var itemId = equipment.ContainsKey(equipmentSlot) ? equipment[equipmentSlot] : null;
        if (string.IsNullOrEmpty(itemId))
        {
          button.HasIcon = false;
          button.HasText = true;
        }
        else
        {
          var item = player.Inventory.ItemRepo.GetItem(itemId);
          if (item == null)
          {
            button.HasIcon = false;
            button.HasText = true;
            return;
          }
          button.HasIcon = true;
          button.HasText = false;
          button.iconSprite = item.Icon;
        }
      }
    }

    public void OnSelect(BaseEventData eventData)
    {
      Debug.Log("Selected! " + gameObject.name);
      equipmentSlot = gameObject.name.ToLower();
      equipmentUI.CurrentSlot = this;
      equipmentUI.slotFilter = equipmentSlot;
      equipmentUI.LoadItems();
    }
  }
}