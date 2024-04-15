using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
  [Serializable]
  public class TabData
  {
    public string Label;
    public Focusable Button;
    public GameObject Content;

    public Action OnClick;
  }
  public class Tabs : MonoBehaviour
  {
    public bool canDeactivate;
    public List<TabData> List = new();
    private readonly Dictionary<string, TabData> TabsDict = new();

    public string CurrentTab;
    public TabData CurrentTabData => TabsDict.ContainsKey(CurrentTab) ? TabsDict[CurrentTab] : null;

    public Form Form;

    public void Awake()
    {
      foreach (var data in List)
      {
        if (TabsDict.ContainsKey(data.Label)) continue;
        data.OnClick = () => ChangeTab(data.Label);
        data.Button.OnClick += data.OnClick;
        TabsDict[data.Label] = data;
      }
    }

    public void OnDestroy()
    {
      foreach (var data in List)
      {
        if (TabsDict.ContainsKey(data.Label))
          TabsDict.Remove(data.Label);
        data.Button.OnClick -= data.OnClick;
      }
    }

    public void ChangeTab(string tabName)
    {
      // we are changing UI visibility, so close menus
      ContextMenuHandler.Instance.CloseAll();
      if (!TabsDict.ContainsKey(tabName)) return;
      if (TabsDict[tabName].Button.IsActivated)
      {
        if (!canDeactivate) return;
        Deactivate(tabName);
        CurrentTab = "";
        return;
      }
      Activate(tabName);

      CurrentTab = tabName;

      Form?.InitializeElements();
      if (Form?.PreviouslySelected != null)
        Form.PreviouslySelected.Activate();
    }

    private void Deactivate(string tabName)
    {
      TabsDict[tabName].Button.IsActivated = false;
      TabsDict[tabName].Content.SetActive(false);
    }

    private void Activate(string tabName)
    {
      foreach (var kvp in TabsDict)
      {
        if (kvp.Key != tabName && kvp.Value.Button.IsActivated)
          Deactivate(kvp.Key);
      }

      TabsDict[tabName].Button.IsActivated = true;
      TabsDict[tabName].Content.SetActive(true);
    }

    public bool IsTabActive(string tabName) => TabsDict[tabName].Button.IsActivated;
  }
}