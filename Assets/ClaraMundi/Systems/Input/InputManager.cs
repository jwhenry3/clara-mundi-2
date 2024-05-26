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
    [HideInInspector]
    public InputActionMap All;

    public void Init()
    {
      Instance = this;
      UI = Instance.InputActionAsset.FindActionMap("UI");
      World = Instance.InputActionAsset.FindActionMap("Player");
      All = Instance.InputActionAsset.FindActionMap("All");
      UI.Enable();
      All.Enable();
      World.Disable();
    }
    private void Awake()
    {
      if (Instance == null)
        Init();
    }
  }
}