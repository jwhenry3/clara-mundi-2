﻿using UnityEngine;

namespace ClaraMundi
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}