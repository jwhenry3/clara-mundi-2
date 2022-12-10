using System;
using UnityEngine;

namespace ClaraMundi
{
    public class PlayerRequiredUI : PlayerUI
    {
        

        protected override void OnPlayerChange(Player _player)
        {
            base.OnPlayerChange(_player);
            if (_player == null)
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
                child.gameObject.SetActive(true);
            }
        }
    }
}