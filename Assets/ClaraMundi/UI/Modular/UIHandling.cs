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
    public static UIHandling Instance;
    public Transform FocusIndicator;
    public PlayerRequiredUI PlayerUI;
    public PlayerRequiredUI NoPlayerUI;
    public GameObject PeersContainer;

    public Transform DebugContainer;

    public GameObject Placeholder;
    public CanvasGroup Backdrop;

    void Start()
    {
      Instance = this;
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
                if (InputUI.IsFocused) return;
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

    void LateUpdate()
    {
      MoveIndicator();
      var lastChild = PeersContainer.transform.GetChild(PeersContainer.transform.childCount - 1);
      if (!lastChild.gameObject.activeInHierarchy)
      {
        lastChild.gameObject.SetActive(true);
      }
      if (InputManager.Instance != null)
      {
        if (AllWindowsClosed())
        {
          InputManager.Instance.World.Enable();
          Backdrop.blocksRaycasts = false;
        }
        else
        {
          InputManager.Instance.World.Disable();
          Backdrop.blocksRaycasts = true;
        }
      }
    }
    public bool AllWindowsClosed()
    {
      return Placeholder.transform.GetSiblingIndex() == Placeholder.transform.parent.childCount - 1;
    }
    void OnQuit(WindowUI window)
    {
      if (PlayerUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
      {
        if (AllWindowsClosed())
        {
          var targeting = PlayerManager.Instance.LocalPlayer?.Targeting;
          if (targeting == null || (targeting.TargetId.Value == null && targeting.SubTargetId == null))
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
              FocusIndicator.gameObject.SetActive(false);
            }
            else
            {
              var wasHidden = !FocusIndicator.gameObject.activeInHierarchy;
              var t = EventSystem.current.currentSelectedGameObject.transform as RectTransform;
              var corners = new Vector3[4];
              t.GetWorldCorners(corners);

              var height = Mathf.Abs(corners[2].y - corners[0].y);
              var destination = new Vector3(
                corners[0].x,
                corners[2].y - height / 2,
                0
              );
              if (wasHidden)
              {
                FocusIndicator.position = destination;
              }
              else
              {
                FocusIndicator.position = Vector3.Slerp(FocusIndicator.position, destination, Time.deltaTime * 20);
              }
              FocusIndicator.gameObject.SetActive(true);
            }
          }
          else
          {
            FocusIndicator.gameObject.SetActive(false);
          }
        }
        else
        {
          FocusIndicator.gameObject.SetActive(false);
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