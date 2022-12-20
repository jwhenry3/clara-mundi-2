using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
    public class InputManager : MonoBehaviour
    {
        public InputActionAsset InputActionAsset;
        public static InputManager Instance;
        [HideInInspector]
        public InputActionMap UI;
        [HideInInspector]
        public InputActionMap World;

        public List<string> InputsFocused = new();
        private void Awake()
        {
            Instance = this;
            UI =  InputManager.Instance.InputActionAsset.FindActionMap("UI");
            World =  InputManager.Instance.InputActionAsset.FindActionMap("Player");
            DontDestroyOnLoad(gameObject);
        }
    }
}