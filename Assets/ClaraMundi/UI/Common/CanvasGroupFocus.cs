using System.Collections;
using System.Collections.Generic;
using GameKit.Dependencies.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class CanvasGroupFocus : MonoBehaviour
  {

    [Header("Buttons")]
    public ButtonWithHybridNav InitialFocus;
    public InputFieldWithHybridNav InitialFocusInput;
    public ButtonWithHybridNav LastFocused;
    public InputFieldWithHybridNav LastFocusInput;
    public Tabs TabsToChange;

    [Header("General Rules")]
    public bool DelayTriggersOnEnable = true;

    public MoveSibling MoveSibling;

    public CanvasGroup canvasGroup
    {
      get;
      private set;
    }

    private bool lastInteractable;

    public void Initialize()
    {
      if (canvasGroup == null)
        canvasGroup = GetComponent<CanvasGroup>();
      if (MoveSibling == null)
        MoveSibling = GetComponent<MoveSibling>();
    }
    public void ClearMemory()
    {
      LastFocused = null;
      LastFocusInput = null;
    }

    void OnEnable()
    {
      Initialize();
      TabsToChange?.ChangeTab(gameObject.name);
      // Debug.Log("Enabled: " + gameObject.name);

      lastInteractable = false;
      StartCoroutine(DelayEnable());
    }

    void OnDisable()
    {
      // Debug.Log("Disabled: " + gameObject.name);
      if (TabsToChange != null)
      {
        if (TabsToChange.CurrentTab == gameObject.name)
          TabsToChange.ChangeTab("");
      }
      if (canvasGroup != null)
        canvasGroup.interactable = false;
    }
    IEnumerator DelayEnable()
    {
      yield return new WaitForSeconds(DelayTriggersOnEnable ? 0.1f : 0);
      if (canvasGroup != null)
        canvasGroup.interactable = true;
    }
    void Update()
    {
      if (canvasGroup == null) return;
      OnInteractableChange();
    }

    void OnInteractableChange()
    {
      if (lastInteractable == canvasGroup.interactable) return;
      lastInteractable = canvasGroup.interactable;
      if (canvasGroup.interactable)
        Select();
    }

    public void Select()
    {
      if (MoveSibling != null)
        MoveSibling.ToFront();
      Debug.Log("SELECT TRIGGERED");
      if (LastFocused != null && LastFocused.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(LastFocused.gameObject, null));
      else if (LastFocusInput != null && LastFocusInput.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(LastFocusInput.gameObject, LastFocusInput));
      else if (InitialFocus != null && InitialFocus.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(InitialFocus.gameObject, null));
      else if (InitialFocusInput != null && InitialFocusInput.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(InitialFocusInput.gameObject, InitialFocusInput));
    }

    IEnumerator DelaySelect(GameObject gameObject, InputFieldWithHybridNav input)
    {
      yield return new WaitForSeconds(0.1f);
      Debug.Log(gameObject);
      if (gameObject != null)
      {
        if (input != null)
        {
          yield return new WaitForSeconds(0.1f);
          input.ActivateInputField();
          EventSystem.current.SetSelectedGameObject(gameObject);
        }
        EventSystem.current.SetSelectedGameObject(gameObject);
      }
    }

  }
}