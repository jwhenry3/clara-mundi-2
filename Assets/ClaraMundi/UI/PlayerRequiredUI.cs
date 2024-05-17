using System;
using UnityEngine;

namespace ClaraMundi
{
  public class PlayerRequiredUI : PlayerUI
  {
    public bool PlayerIsRequired;
    public bool IsDebug;

    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if (IsDebug)
      {
        ShowAll();
        return;
      }
      if ((_player == null && PlayerIsRequired) || (_player != null && !PlayerIsRequired))
        HideAll();
      else
        ShowAll();
    }
    private void OnEnable()
    {
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