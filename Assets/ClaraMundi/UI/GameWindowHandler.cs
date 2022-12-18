using System;
using UnityEngine;

namespace ClaraMundi
{
    public class GameWindowHandler : MonoBehaviour
    {
        public static GameWindowHandler Instance;

        public Tabs Tabs;

        private void Awake()
        {
            Instance = this;
            Tabs = GetComponent<Tabs>();
        }
    }
}