using System;
using UnityEngine;

namespace ClaraMundi
{
  public class PlayerRequiredUI : PlayerUI
  {
    public bool PlayerIsRequired;

    protected override void OnPlayerChange(Player _player)
    {
      base.OnPlayerChange(_player);
      if ((_player == null && PlayerIsRequired) || (_player != null && !PlayerIsRequired))
        HideAll();
      else
        ShowAll();
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
        Debug.Log(child.gameObject.name);
        child.gameObject.SetActive(true);
      }
    }
  }
}