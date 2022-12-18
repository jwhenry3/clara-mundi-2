using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [Serializable]
    public class TabData
    {
        public string Label;
        public UIAnimator Button;
        public UIAnimator Content;
        
        public Action OnClick;
    }
    public class Tabs : MonoBehaviour
    {
        public bool canDeactivate;
        public List<TabData> List = new();
        private readonly Dictionary<string, TabData> TabsDict = new();
        
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
            if (!TabsDict.ContainsKey(tabName)) return;
            if (TabsDict[tabName].Button.IsActivated())
            {
                if (!canDeactivate) return;
                Deactivate(tabName);
                return;
            }
            Activate(tabName);
        }

        private void Deactivate(string tabName)
        {
            TabsDict[tabName].Button.Deactivate();
            TabsDict[tabName].Content.Hide();
        }

        private void Activate(string tabName)
        {
            foreach (var kvp in TabsDict)
            {
                if (kvp.Key != tabName && kvp.Value.Button.IsActivated())
                    Deactivate(kvp.Key);
            }
            TabsDict[tabName].Button.Activate();
            TabsDict[tabName].Content.Show();
        }

        public bool IsTabActive(string tabName) => TabsDict[tabName].Button.IsActivated();
    }
}