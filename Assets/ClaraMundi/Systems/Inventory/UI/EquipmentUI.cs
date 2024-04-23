using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class EquipmentUI : PlayerUI
  {
    OwningEntityHolder owner = new();

    public ContextMenu ItemContextMenu;

    public ItemTooltipUI Tooltip;

    public GameObject Grid;


    public override void Start()
    {
      if (Grid != null)
      {
        foreach (Transform child in Grid.transform)
        {
          ItemUI item = child.GetComponent<ItemUI>();
          if (item != null)
          {
            item.Tooltip = Tooltip;
            item.ContextMenu = ItemContextMenu;
            item.EquipmentSlot = item.gameObject.name.ToLower();
            item.EquipmentUI = this;
            item.ShowEquippedStatus = false;
          }
        }
      }
      base.Start();
    }

    public void OnEnable()
    {
      if (ItemManager.Instance == null) return;
      if (PlayerManager.Instance != null)
        OnPlayerChange(PlayerManager.Instance.LocalPlayer);
    }

    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if (entity == null) return;
      owner.SetEntity(_player.Entity);
    }

    public void CloseContextMenu()
    {
      EventSystem.current.SetSelectedGameObject(ItemContextMenu.ContextualItem?.gameObject);
      ItemContextMenu.ContextualItem = null;
      ItemContextMenu.gameObject.SetActive(false);
    }
  }
}