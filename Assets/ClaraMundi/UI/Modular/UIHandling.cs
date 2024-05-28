using System;
using System.Collections;
using System.Collections.Generic;
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

    public List<WindowUI> windows;

    void Start()
    {
      Instance = this;
      ActionMenuUI.Instance = PeersContainer.GetComponentInChildren<ActionMenuUI>(true);
      foreach (WindowUI window in windows)
      {
        window.SetUp();
        if (string.IsNullOrEmpty(window.TriggerAction)) continue;
        if (window.TriggerAction == "Quit")
          InputManager.Instance.UI.FindAction("Cancel").performed += (context) => OnQuit(window);
        else
          TryAction(window);
      }
    }

    void TryAction(WindowUI window)
    {
      try
      {
        InputManager.Instance.UI.FindAction(window.TriggerAction).performed += (context) =>
        {
          if (InputUI.IsFocused) return;
          if (PlayerUI.IsDebug || (PlayerManager.Instance != null && PlayerManager.Instance.LocalPlayer != null))
          {
            Placeholder.transform.SetAsLastSibling();
            window.moveSibling.ToFront();
          }
        };
      }
      catch (NullReferenceException e)
      {
        Debug.LogWarning(e);
      }
    }

    void OnEnable()
    {
      foreach (Transform child in DebugContainer)
        child.gameObject.SetActive(PlayerUI.IsDebug || NoPlayerUI.IsDebug);
      PlayerUI.gameObject.SetActive(true);
      NoPlayerUI.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
      MoveIndicator();
      var lastChild = PeersContainer.transform.GetChild(PeersContainer.transform.childCount - 1);
      if (!lastChild.gameObject.activeInHierarchy)
        lastChild.gameObject.SetActive(true);
      Backdrop.blocksRaycasts = !AllWindowsClosed();
      if (InputManager.Instance == null) return;
      if (!Backdrop.blocksRaycasts)
      {
        InputManager.Instance.World.Enable();
        InputManager.Instance.Actions.Enable();
      }
      else
      {
        InputManager.Instance.World.Disable();
        InputManager.Instance.Actions.Disable();
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
        if (!Backdrop.blocksRaycasts)
        {
          var targeting = PlayerManager.Instance.LocalPlayer?.Targeting;
          if (targeting == null || (targeting.TargetId.Value == null && targeting.SubTargetId.Value == null))
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
      if (FocusIndicator == null) return;
      if (EventSystem.current == null) return;
      var selected = EventSystem.current.currentSelectedGameObject;
      if (selected != null && selected.activeInHierarchy)
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
          FocusIndicator.position = destination;
        else
          FocusIndicator.position = Vector3.Slerp(FocusIndicator.position, destination, Time.deltaTime * 20);
        FocusIndicator.gameObject.SetActive(true);
        return;
      }
      FocusIndicator.gameObject.SetActive(false);
    }

    public void MoveLastSiblingBack()
    {
      if (PeersContainer != null)
        PeersContainer.transform.GetChild(PeersContainer.transform.childCount - 1).SetAsFirstSibling();
    }
  }

}