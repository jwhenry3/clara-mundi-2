using System;
using UnityEngine;

namespace ClaraMundi
{
  public class PlayerRequiredUI : PlayerUI
  {
    public bool PlayerIsRequired;
    public bool IsDebug;
    public bool IsManual;

    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if (!IsManual)
      {
        UpdateVisibility();
      }
    }

    public void UpdateVisibility()
    {
      if (IsDebug)
      {
        ShowAll();
        return;
      }
      if ((player == null && PlayerIsRequired) || (player != null && !PlayerIsRequired))
        HideAll();
      else
        ShowAll();
    }
    private void OnEnable()
    {
      if (!IsManual)
        OnPlayerChange(player);
    }
    private void OnDisable()
    {
      HideAll();
    }

    private void HideAll()
    {
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(false);
      }
    }

    private void ShowAll()
    {
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(true);
      }
    }
  }
}