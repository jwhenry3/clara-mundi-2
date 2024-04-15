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
    public List<TabData> List;
    private Dictionary<string, TabData> TabsDict = new();

    public string CurrentTab;
    public TabData CurrentTabData => TabsDict.ContainsKey(CurrentTab) ? TabsDict[CurrentTab] : null;

    public Form Form;

    public void Start()
    {
      if (List == null) return;
      foreach (var data in List)
      {
        Debug.Log("Check Dictionary: " + gameObject.name + " - " + data.Label);
        if (TabsDict.ContainsKey(data.Label)) continue;
        Debug.Log("Attach Listener: " + gameObject.name + " - " + data.Label);
        data.Button.OnClick += () => ChangeTab(data.Label);
        TabsDict[data.Label] = data;
      }
    }

    public void OnDestroy()
    {
      foreach (var data in List)
      {
        Debug.Log("Dettach Listener: " + gameObject.name + " - " + data.Label);
        data.Button.OnClick -= data.OnClick;
      }
      TabsDict = new();
    }

    public void ChangeTab(string tabName)
    {
      Debug.Log("Change Tab: " + tabName);
      // we are changing UI visibility, so close menus
      ContextMenuHandler.Instance.CloseAll();
      if (tabName == "")
      {
        Deactivate(CurrentTab);
        return;
      }
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
    }

    private void Deactivate(string tabName)
    {
      if (tabName == "") return;
      TabsDict[tabName].Button.IsActivated = false;
      TabsDict[tabName].Content.SetActive(false);
    }

    private void Activate(string tabName)
    {
      if (CurrentTab != tabName)
        Deactivate(CurrentTab);

      TabsDict[tabName].Button.IsActivated = true;
      TabsDict[tabName].Content.SetActive(true);
    }

    public bool IsTabActive(string tabName) => TabsDict[tabName].Button.IsActivated;
  }
}