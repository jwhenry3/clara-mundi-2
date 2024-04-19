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
    public string Name;
    public ContextMenuItem ContextMenuItemPrefab;
    public ContextMenuItemData[] MenuItems;

    readonly Dictionary<string, ContextMenuItem> Options = new();
    private readonly List<string> disabledItems = new();

    public Form Form;

    public string CurrentValue;

    public Action OnClose;

    private void Start()
    {
      Form = Form ?? GetComponent<Form>();
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
    }

    private void OnEnable()
    {
      if (ContextMenuHandler.Instance.GroupsToDisableOnOpen != null)
        ContextMenuHandler.Instance.GroupsToDisableOnOpen.ForEach(group => group.interactable = false);
      SelectFirstElement();
    }

    public void SelectFirstElement()
    {
      foreach (var kvp in Options)
      {
        var child = kvp.Value;
        if (!child.gameObject.activeSelf) continue;
        if (CurrentValue?.Length > 0 && kvp.Key != CurrentValue) continue;
        Debug.Log("Select Element");
        EventSystem.current.SetSelectedGameObject(child.gameObject);
        break;
      }
    }

    public void OnDisable()
    {
      if (ContextMenuHandler.Instance.GroupsToDisableOnOpen != null)
        ContextMenuHandler.Instance.GroupsToDisableOnOpen.ForEach(group => group.interactable = true);
      if (ContextMenuHandler.Instance.ContextualFormElement != null)
        EventSystem.current.SetSelectedGameObject(ContextMenuHandler.Instance.ContextualFormElement.gameObject);
      ContextMenuHandler.Instance.ContextualItem = null;
      OnClose?.Invoke();
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