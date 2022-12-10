using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace ClaraMundi
{
    public class ContextMenus : MonoBehaviour
    {
        public GameObject Backdrop;
        [ShowInInspector]
        public List<ContextMenu> menus = new();

        public void ShowMenu(string key, Vector2 position)
        {
            var menu = menus.Find((m) => m.Name == key);
            if (menu == null) return;
            menu.gameObject.SetActive(true);
            menu.transform.position = position;
        }
        public void HideMenus()
        {
            foreach (var menu in menus)
                menu.gameObject.SetActive(false);
        }

    }
}