using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ClaraMundi
{
  public class UIHandling : MonoBehaviour
  {
    public bool IsDebug;
    public GameObject MainUI;

    public GameObject Placeholder;

    void OnEnable()
    {
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(IsDebug);
      }
      MainUI.SetActive(true);
      if (InputManager.Instance == null) return;
      foreach (WindowUI window in MainUI.GetComponentsInChildren<WindowUI>(true))
      {
        window.SetUp();
        if (!string.IsNullOrEmpty(window.TriggerAction))
        {
          if (window.TriggerAction == "Quit")
          {
            InputManager.Instance.UI.FindAction("Cancel").performed += (context) => OnQuit(window);
          }
          else
          {
            try
            {
              InputManager.Instance.UI.FindAction(window.TriggerAction).performed += (context) =>
              {
                if (IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
                  window.moveSibling.ToFront();
              };
            }
            catch (NullReferenceException e)
            {
              Debug.LogWarning(e);
            }
          }
        }
      }
    }

    void LateUpdate()
    {
      if (InputManager.Instance != null)
      {
        if (Placeholder.transform.GetSiblingIndex() == Placeholder.transform.parent.childCount - 1)
          InputManager.Instance.World.Enable();
        else
          InputManager.Instance.World.Disable();
      }
    }

    void OnQuit(WindowUI window)
    {
      if (IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
      {
        if (Placeholder.transform.GetSiblingIndex() == Placeholder.transform.parent.childCount - 1)
        {
          window.moveSibling.ToFront();
        }
      }
    }
  }
}