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
    public static Dictionary<string, CanvasGroupFocus> Controls = new();

    [Header("Information")]
    public string ControlName;
    [Header("Buttons")]
    public ButtonWithHybridNav InitialFocus;
    public InputFieldWithHybridNav InitialFocusInput;
    public ButtonWithHybridNav LastFocused;
    public InputFieldWithHybridNav LastFocusInput;
    [Header("Input Rules")]
    public InputActionAsset InputActionAsset;
    public CanvasGroup CloseOnCancel;
    public CanvasGroup DisableOnCancel;
    public Tabs TabsToChangeOnCancel;

    [Header("General Rules")]

    public CanvasGroup[] DisableOnEnable;
    public CanvasGroup[] HideOnEnable;
    public CanvasGroup[] KeepEnabledWhileEnabled;
    public CanvasGroup[] KeepVisibleWhileEnabled;
    public CanvasGroup[] EnableOnDisable;
    public CanvasGroup[] ShowOnDisable;
    public CanvasGroup[] HideOnDisable;
    public CanvasGroup[] DisableOnDisable;

    private InputAction cancelAction;
    public CanvasGroup canvasGroup
    {
      get;
      private set;
    }

    private bool lastInteractable;


    void OnEnable()
    {
      canvasGroup = GetComponent<CanvasGroup>();
      Debug.Log("Enabled: " + gameObject.name);

      if (InputActionAsset != null)
      {
        if (CloseOnCancel != null || DisableOnCancel != null)
        {
          cancelAction = InputActionAsset.FindAction("UI/Cancel");
          cancelAction.performed += OnCancel;
        }
      }
      lastInteractable = false;
      StartCoroutine(DelayEnable());
    }

    void OnDisable()
    {
      Debug.Log("Disabled: " + gameObject.name);
      if (cancelAction != null)
        cancelAction.performed -= OnCancel;
      foreach (var group in KeepEnabledWhileEnabled)
        group.interactable = false;
      foreach (var group in EnableOnDisable)
        group.interactable = true;
      foreach (var group in DisableOnDisable)
        group.interactable = false;
      foreach (var obj in ShowOnDisable)
        obj.gameObject.SetActive(true);
      foreach (var obj in HideOnDisable)
        obj.gameObject.SetActive(false);
    }
    IEnumerator DelayEnable()
    {
      yield return new WaitForSeconds(0.1f);
      foreach (var group in DisableOnEnable)
        group.interactable = false;
      foreach (var group in KeepEnabledWhileEnabled)
        group.interactable = true;

      foreach (var obj in HideOnEnable)
        obj.gameObject.SetActive(false);
      foreach (var obj in KeepVisibleWhileEnabled)
        obj.gameObject.SetActive(true);
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (canvasGroup == null || canvasGroup.interactable == true)
      {
        CloseOnCancel?.gameObject.SetActive(false);
        if (DisableOnCancel != null)
          DisableOnCancel.interactable = false;
        if (TabsToChangeOnCancel != null)
          TabsToChangeOnCancel.ChangeTab("");
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
      if (LastFocused != null)
        StartCoroutine(DelaySelect(LastFocused.gameObject));
      else if (LastFocusInput != null)
        StartCoroutine(DelaySelect(LastFocusInput.gameObject));
      else if (InitialFocus != null)
        StartCoroutine(DelaySelect(InitialFocus.gameObject));
      else if (InitialFocusInput != null)
        StartCoroutine(DelaySelect(InitialFocusInput.gameObject));
    }

    IEnumerator DelaySelect(GameObject gameObject)
    {
      yield return new WaitForSeconds(0.1f);
      EventSystem.current.SetSelectedGameObject(gameObject);
    }

  }
}