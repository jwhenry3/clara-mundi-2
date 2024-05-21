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

    public void Init()
    {
      Instance = this;
      UI = Instance.InputActionAsset.FindActionMap("UI");
      World = Instance.InputActionAsset.FindActionMap("Player");
      UI.Enable();
      World.Disable();
    }
    private void Awake()
    {
      if (Instance == null)
        Init();
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