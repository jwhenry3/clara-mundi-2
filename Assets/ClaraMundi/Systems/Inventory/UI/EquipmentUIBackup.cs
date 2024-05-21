using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class EquipmentUIBackup : PlayerUI
  {
    OwningEntityHolder owner = new();

    public InventoryUIBackup InventoryUI;

    public ContextMenu ItemContextMenu;

    public ItemTooltipUI Tooltip;

    public GameObject Grid;


    public override void Start()
    {
      if (Grid != null)
      {
        foreach (ItemUI item in Grid.GetComponentsInChildren<ItemUI>())
        {
          item.Tooltip = Tooltip;
          item.InventoryUI = InventoryUI;
          item.ContextMenu = ItemContextMenu;
          item.EquipmentSlot = item.gameObject.name.ToLower();
          item.EquipmentUI = this;
          item.ShowEquippedStatus = false;
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
      foreach (ItemUI item in Grid.GetComponentsInChildren<ItemUI>())
        item.SetOwner(owner);
      if (entity == null) return;
      owner.SetEntity(_player.Entity);
    }
  }
}