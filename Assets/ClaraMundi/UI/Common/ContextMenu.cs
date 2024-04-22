using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using Unity.Collections;
using System.Collections;
using Sirenix.Utilities;
using System.Text.RegularExpressions;

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
    public static ContextMenu OpenedContextMenu;

    readonly Dictionary<string, ContextMenuItem> Options = new();


    public Action OnClose;

    [Header("State")]
    public string Name;
    public string CurrentValue;

    [Header("Programmatic Options")]
    public ContextMenuItem ContextMenuItemPrefab;
    public ContextMenuItemData[] MenuItems;

    [Header("Context Menu Subjects")]
    public GameObject ContextualGameObject;
    public ItemUI ContextualItem;
    public Player ContextualPlayer;
    public string ContextualText;

    private void Start()
    {
      if (MenuItems.Length == 0)
      {
        foreach (ContextMenuItem item in GetComponentsInChildren<ContextMenuItem>())
          Options.Add(item.Label.text, item);
      }
      if (Options.Count != 0) return;
      foreach (var item in MenuItems)
      {
        var instance = Instantiate(ContextMenuItemPrefab, transform, false);
        instance.Label.text = item.Label;
        instance.Data = item;
        // close the menu when the menu item has been clicked
        instance.Data.OnClick.AddListener(() => gameObject.SetActive(false));
        Options.Add(item.Label, instance);
      }
    }

    private void OnEnable()
    {
      if (OpenedContextMenu != null)
      {
        OpenedContextMenu.gameObject.SetActive(false);
        OpenedContextMenu = null;
      }
      OpenedContextMenu = this;
      SelectFirstElement();
    }

    public void SelectFirstElement()
    {

      foreach (ContextMenuItem item in GetComponentsInChildren<ContextMenuItem>())
      {
        if (CurrentValue != "" && item.Label.text != CurrentValue) continue;
        EventSystem.current.SetSelectedGameObject(item.gameObject);
        break;
      }
    }

    public void OnDisable()
    {
      if (OpenedContextMenu == this)
        OpenedContextMenu = null;
      OnClose?.Invoke();
    }

    public void SetItemActive(string itemName, bool value)
    {
      if (Options.ContainsKey(itemName))
        Options[itemName].gameObject.SetActive(value);
    }

    public void ChangeLabelOf(string itemName, string label)
    {
      if (!Options.ContainsKey(itemName)) return;
      Options[itemName].Label.text = label;

    }

  }
}