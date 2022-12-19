using System;
using UnityEngine;

namespace ClaraMundi
{
    public class ContextMenuHandler : MonoBehaviour
    {
        public ItemUI ContextualItem;
        public Player ContextualPlayer;
        public ContextMenu PlayerMenu;
        public ContextMenu ItemMenu;
        public ContextMenu EquippedMenu;

        public static ContextMenuHandler Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void CloseAll()
        {
            ContextualItem = null;
            ContextualPlayer = null;
            ItemMenu.gameObject.SetActive(false);
            PlayerMenu.gameObject.SetActive(false);
            EquippedMenu.gameObject.SetActive(false);
        }
    }
}