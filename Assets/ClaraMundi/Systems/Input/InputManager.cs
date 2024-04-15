using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private void Awake()
    {
      Instance = this;
      UI = Instance.InputActionAsset.FindActionMap("UI");
      World = Instance.InputActionAsset.FindActionMap("Player");
      UI.Enable();
      World.Disable();
    }
    public static bool IsFocusedOnInput()
    {
      var obj = EventSystem.current.currentSelectedGameObject;
      if (obj == null) return false;
      var input = obj.GetComponent<TMP_InputField>();
      return input != null && input.isFocused;
    }
  }
}