using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaraMundi
{
  [Serializable]
  public struct AllInputOption
  {

    public GameObject[] Show;
    public GameObject[] Hide;
  }

  [Serializable]
  public struct InputOption
  {
    public string InputAction;

    public GameObject[] Show;
    public MoveSibling[] MoveToFront;
    public GameObject[] Hide;
    public MoveSibling[] MoveToBackIfOpenOrder;
    public GameObject[] HideIfOpenOrder;
  }
  public class GameWindowInput : MonoBehaviour
  {
    public static GameWindowInput Instance;
    public InputActionAsset InputActionAsset;

    public AllInputOption All;
    public InputOption[] Options;
    private Dictionary<string, InputOption> optionsDict;
    void Start()
    {
      Instance = this;

      optionsDict = new();
      foreach (var option in Options)
      {
        optionsDict[option.InputAction] = option;
      }
    }
    void OnEnable()
    {
      foreach (var option in Options)
        InputActionAsset.FindAction("UI/" + option.InputAction).performed += OnPerformed;
    }
    void OnDisable()
    {
      foreach (var option in Options)
        InputActionAsset.FindAction("UI/" + option.InputAction).performed -= OnPerformed;
    }

    void OnPerformed(InputAction.CallbackContext context)
    {
      var action = context.action.name;
      Trigger(action);
    }
    public void Trigger(string action)
    {
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      if (action != "Cancel")
        if (InputFieldWithHybridNav.CurrentInput != null) return; // do not receive input if typing
      if (optionsDict.ContainsKey(action))
      {
        foreach (GameObject obj in optionsDict[action].Hide)
          obj.SetActive(false);
        // when only one window is closed at a time, we don't close "all" objects
        // this is intended for a "cancel" action that closes the top-most window at a time
        if (optionsDict[action].HideIfOpenOrder.Length == 0 && optionsDict[action].MoveToBackIfOpenOrder.Length == 0)
        {
          foreach (GameObject obj in All.Hide)
            obj.SetActive(false);

          foreach (GameObject obj in All.Show)
            obj.SetActive(true);
        }
        foreach (MoveSibling obj in optionsDict[action].MoveToFront)
          obj.ToFront();
        foreach (GameObject obj in optionsDict[action].Show)
          obj.SetActive(true);
        bool movedToBack = false;
        foreach (MoveSibling sib in optionsDict[action].MoveToBackIfOpenOrder)
        {
          if (sib.IsInFront())
          {
            movedToBack = true;
            sib.ToBack();
            break;
          }
        }
        if (!movedToBack)
        {
          foreach (GameObject obj in optionsDict[action].HideIfOpenOrder)
          {
            if (!obj.activeInHierarchy) continue;
            obj.SetActive(false);
            break;
          }
        }
      }
    }
    public void Update()
    {
      if (InputManager.Instance != null)
      {
        if (InputFieldWithHybridNav.CurrentInput != null || ButtonWithHybridNav.CurrentButton != null)
        {
          if (InputManager.Instance.World.enabled)
          {
            Debug.Log("DISABLE");
            InputManager.Instance.World.Disable();
          }
          return;
        }
      }
      if (!InputManager.Instance.World.enabled)
      {
        Debug.Log("ENABLE");
        InputManager.Instance.World.Enable();
      }
    }
  }
}