using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

namespace ClaraMundi
{
  [Serializable]
  public struct InputOption
  {
    public string InputAction;
    public UnityEvent Event;
  }
  public class GameWindowInput : MonoBehaviour
  {
    public InputActionAsset InputActionAsset;

    public InputOption[] Options;
  }
}