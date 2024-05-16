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
              InputManager.Instance.UI.FindAction(window.TriggerAction).performed += (context) => window.moveSibling.ToFront();
            }
            catch (NullReferenceException e)
            {
              Debug.LogWarning(e);
            }
          }
        }
      }
    }

    void OnQuit(WindowUI window)
    {
      if (Placeholder.transform.GetSiblingIndex() == Placeholder.transform.parent.childCount - 1)
      {
        window.moveSibling.ToFront();
      }
    }
  }
}