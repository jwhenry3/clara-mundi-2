using System;
using System.Collections;
using GameKit.Dependencies.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace ClaraMundi
{
  public class UIHandling : MonoBehaviour
  {
    public Transform FocusIndicator;
    public PlayerRequiredUI PlayerUI;
    public PlayerRequiredUI NoPlayerUI;
    public GameObject PeersContainer;

    public Transform DebugContainer;

    public GameObject Placeholder;

    private float tick;
    private float interval = 0.1f;

    void Start()
    {
      foreach (WindowUI window in PlayerUI.GetComponentsInChildren<WindowUI>(true))
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
                if (PlayerUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
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
        child.gameObject.SetActive(PlayerUI.IsDebug || NoPlayerUI.IsDebug);
      }
      PlayerUI.gameObject.SetActive(true);
      NoPlayerUI.gameObject.SetActive(true);
    }
    void Update()
    {
      MoveIndicator();
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
      if (PlayerUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
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

    void MoveIndicator()
    {
      if (FocusIndicator != null)
      {
        if (EventSystem.current != null)
        {
          if (EventSystem.current.currentSelectedGameObject != null)
          {
            if (!EventSystem.current.currentSelectedGameObject.activeInHierarchy)
            {
              EventSystem.current.SetSelectedGameObject(null);
              FocusIndicator.gameObject.SetActive(false);
              FocusIndicator.position = Vector3.one * -100000;
            }
            else
            {
              var t = EventSystem.current.currentSelectedGameObject.transform as RectTransform;
              var corners = new Vector3[4];
              t.GetWorldCorners(corners);

              var height = Mathf.Abs(corners[2].y - corners[0].y);
              FocusIndicator.position = Vector3.Slerp(FocusIndicator.position, new Vector3(
                corners[0].x,
                corners[2].y - height / 2,
                0
              ), Time.deltaTime * 20);
              FocusIndicator.gameObject.SetActive(true);
            }
          }
          else
          {
            FocusIndicator.gameObject.SetActive(false);
            FocusIndicator.position = Vector3.one * -100000;
          }
        }
        else
        {
          FocusIndicator.gameObject.SetActive(false);
          FocusIndicator.position = Vector3.one * -100000;
        }
      }
    }

    public void MoveLastSiblingBack()
    {
      if (PeersContainer != null)
      {
        PeersContainer.transform.GetChild(PeersContainer.transform.childCount - 1).SetAsFirstSibling();
      }
    }
  }

}