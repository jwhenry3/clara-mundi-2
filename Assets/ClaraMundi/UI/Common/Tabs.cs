using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public bool AllowMultiple;
        private MoveToFront MoveToFront;
        public List<TabData> List = new();
        private readonly Dictionary<string, TabData> TabsDict = new();
        private bool inCurrentWindow;
        public void Awake()
        {
            MoveToFront = GetComponentInParent<MoveToFront>();
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
            if (MoveToFront != null)
                MoveToFront.Move();
            // we are changing UI visibility, so close menus
            ContextMenuHandler.Instance.CloseAll();
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
            if (!AllowMultiple)
            {
                foreach (var kvp in TabsDict)
                {
                    if (kvp.Key != tabName && kvp.Value.Button.IsActivated())
                        Deactivate(kvp.Key);
                }
            }

            TabsDict[tabName].Button.Activate();
            TabsDict[tabName].Content.Show();
            var contentMove = TabsDict[tabName].Content.GetComponentInParent<MoveToFront>();
            if (contentMove != null)
                contentMove.Move();
        }

        public bool IsTabActive(string tabName) => TabsDict[tabName].Button.IsActivated();
    }
}