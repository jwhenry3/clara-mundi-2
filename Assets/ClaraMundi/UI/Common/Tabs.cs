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
    }
    public class Tabs : MonoBehaviour
    {
        public bool canDeactivate;
        public List<TabData> List = new();

        public void Start()
        {
            foreach (var data in List)
            {
                data.Button.OnClick += () => ChangeTab(data.Label);
            }
        }

        public void ChangeTab(string tabName)
        {
            foreach (TabData data in List)
            {
                if (data.Label == tabName && (!canDeactivate || !data.Button.IsActivated()))
                {
                    data.Button.Activate();
                    data.Content.Show();
                }
                else
                {
                    data.Button.Deactivate();
                    data.Content.Hide();
                }
            }
        }
    }
}