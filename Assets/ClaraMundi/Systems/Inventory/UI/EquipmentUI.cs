using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class EquipmentUI : PlayerUI
  {
    OwningEntityHolder owner = new();

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
      if (ContextMenuHandler.Instance == null) return;
      EventSystem.current.SetSelectedGameObject(ContextMenuHandler.Instance.ContextualItem?.gameObject);
      ContextMenuHandler.Instance.ContextualItem = null;
      ContextMenuHandler.Instance.ItemMenu.gameObject.SetActive(false);
    }
  }
}