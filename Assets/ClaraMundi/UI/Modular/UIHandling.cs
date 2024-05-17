using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ClaraMundi
{
  public class UIHandling : MonoBehaviour
  {
    public PlayerRequiredUI PlayerRequiredUI;
    public GameObject MainUI;

    public Transform DebugContainer;

    public GameObject Placeholder;



    void Start()
    {
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
                if (PlayerRequiredUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
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
    void OnEnable()
    {
      foreach (Transform child in DebugContainer)
      {
        child.gameObject.SetActive(PlayerRequiredUI.IsDebug);
      }
      MainUI.SetActive(PlayerRequiredUI.IsDebug || PlayerManager.Instance?.LocalPlayer != null);
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
      if (PlayerRequiredUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
      {
        if (Placeholder.transform.GetSiblingIndex() == Placeholder.transform.parent.childCount - 1)
        {
          window.moveSibling.ToFront();
        }
      }
    }
    public void Quit()
    {
      // save any game data here
#if UNITY_EDITOR
      // Application.Quit() does not work in the editor so
      // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
      UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
  }
}