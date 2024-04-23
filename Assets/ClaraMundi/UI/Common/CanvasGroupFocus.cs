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
    [Header("Input Rules")]
    public InputActionAsset InputActionAsset;
    public CanvasGroup CloseOnCancel;
    public CanvasGroup DisableOnCancel;
    public Tabs TabsToChange;

    [Header("General Rules")]
    public bool DelayTriggersOnEnable = true;

    public CanvasGroup[] DisableOnEnable;
    public CanvasGroup[] HideOnEnable;
    public CanvasGroup[] EnableOnEnable;
    public CanvasGroup[] ShowOnEnable;
    public CanvasGroup[] EnableOnDisable;
    public CanvasGroup[] ShowOnDisable;
    public CanvasGroup[] HideOnDisable;
    public CanvasGroup[] DisableOnDisable;

    public MoveSibling[] MoveToFrontOnEnable;

    private InputAction cancelAction;

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
    }

    void OnEnable()
    {
      Initialize();
      TabsToChange?.ChangeTab(gameObject.name);
      // Debug.Log("Enabled: " + gameObject.name);

      if (InputActionAsset != null)
      {
        if (CloseOnCancel != null || DisableOnCancel != null)
        {
          cancelAction = InputActionAsset.FindAction("UI/Cancel");
          cancelAction.performed += OnCancel;
        }
      }
      if (MoveToFrontOnEnable != null)
      {
        foreach (var sibling in MoveToFrontOnEnable)
          sibling.ToFront();
      }
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
      if (cancelAction != null)
        cancelAction.performed -= OnCancel;
      foreach (var group in EnableOnDisable)
      {
        if (group.gameObject.activeInHierarchy)
          group.interactable = true;
      }
      foreach (var group in DisableOnDisable)
        group.interactable = false;
      foreach (var obj in ShowOnDisable)
        obj.gameObject.SetActive(true);
      foreach (var obj in HideOnDisable)
        obj.gameObject.SetActive(false);
    }
    IEnumerator DelayEnable()
    {
      yield return new WaitForSeconds(DelayTriggersOnEnable ? 0.1f : 0);
      if (canvasGroup != null)
        canvasGroup.interactable = true;
      foreach (var group in DisableOnEnable)
        group.interactable = false;
      foreach (var group in EnableOnEnable)
        group.interactable = true;

      foreach (var obj in HideOnEnable)
        obj.gameObject.SetActive(false);
      foreach (var obj in ShowOnEnable)
        obj.gameObject.SetActive(true);
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (canvasGroup == null || canvasGroup.interactable == true)
      {
        CloseOnCancel?.gameObject.SetActive(false);
        if (DisableOnCancel != null)
          DisableOnCancel.interactable = false;
        if (TabsToChange != null)
          TabsToChange.ChangeTab("");
      }
    }

    void Update()
    {
      if (canvasGroup == null) return;
      OnInteractableChange();
    }

    void OnInteractableChange()
    {
      if (!canvasGroup.interactable)
      {
        lastInteractable = canvasGroup.interactable;
        return;
      }
      if (lastInteractable == canvasGroup.interactable) return;
      lastInteractable = canvasGroup.interactable;
      Select();
    }

    public void Select()
    {
      if (LastFocused != null && LastFocused.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(LastFocused.gameObject));
      else if (LastFocusInput != null && LastFocusInput.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(LastFocusInput.gameObject));
      else if (InitialFocus != null && InitialFocus.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(InitialFocus.gameObject));
      else if (InitialFocusInput != null && InitialFocusInput.gameObject.activeInHierarchy)
        StartCoroutine(DelaySelect(InitialFocusInput.gameObject));
    }

    IEnumerator DelaySelect(GameObject gameObject)
    {
      yield return new WaitForSeconds(0.1f);
      if (gameObject != null)
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Show(InputAction.CallbackContext context)
    {
      // Debug.Log("Show " + gameObject.name);
      Show();
    }
    public void Hide(InputAction.CallbackContext context)
    {
      Hide();
    }
    public void Show()
    {
      gameObject.SetActive(true);
    }
    public void Hide()
    {
      gameObject.SetActive(false);
    }

  }
}