using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

namespace ClaraMundi
{
    [Serializable]
    public class ContextMenuItemData
    {
        public string Label;
        public UnityEvent OnClick = new();
    }
    public class ContextMenu : MonoBehaviour
    {
        public string Name;
        public ContextMenuItem ContextMenuItemPrefab;
        public ContextMenuItemData[] MenuItems;

        readonly Dictionary<string, ContextMenuItem> Options = new();
        private readonly List<string> disabledItems = new();

        private void Start()
        {
            if (Options.Count != 0) return;
            foreach (var item in MenuItems)
            {
                var instance = Instantiate(ContextMenuItemPrefab, transform, false);
                instance.Label.text = item.Label;
                instance.gameObject.SetActive(!disabledItems.Contains(item.Label));
                instance.Data = item;
                // close the menu when the menu item has been clicked
                instance.Data.OnClick.AddListener(() => gameObject.SetActive(false));
                Options.Add(item.Label, instance);
            }
            SelectFirstElement();
        }

        private void OnEnable()
        {
            SelectFirstElement();
        }

        public void SelectFirstElement()
        {
            foreach (ContextMenuItem child in GetComponentsInChildren<ContextMenuItem>())
            {
                if (!child.gameObject.activeSelf) continue;
                EventSystem.current.SetSelectedGameObject(child.gameObject);
                break;
            }
        }

        public void OnDisable()
        {
            foreach (Transform child in transform)
            {
                var autoFocus = child.GetComponent <AutoFocus>();
                if (autoFocus != null)
                    autoFocus.enabled = false;
            }
        }

        public void SetItemActive(string itemName, bool value)
        {
            if (Options.ContainsKey(itemName))
                Options[itemName].gameObject.SetActive(value);
            if (value)
            {
                if (disabledItems.Contains(itemName))
                    disabledItems.Remove(itemName);
            }
            else
                if (!disabledItems.Contains(itemName))
                    disabledItems.Add(itemName);
        }

        public void ChangeLabelOf(string itemName, string label)
        {
            if (!Options.ContainsKey(itemName)) return;
            Options[itemName].Label.text = label;
            
        }
    }
}