using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace ClaraMundi
{

  [Serializable]
  public class ControlsDictionary : UnitySerializedDictionary<string, CanvasGroupFocus> { }
  public class TopLevelCanvas : PlayerUI
  {
    public EventSystem EventSystem;
    public static TopLevelCanvas Instance;
    public bool IsDebug;
    public GameObject Container;
    public InputActionAsset InputActionAsset;

    public ControlsDictionary Controls;

    public ChatWindowUI Chat;
    public override void Start()
    {
      base.Start();
      Instance = this;
      EventSystem.gameObject.SetActive(IsDebug);
      Container.SetActive(IsDebug);
    }

  }
}