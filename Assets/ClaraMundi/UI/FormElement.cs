﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ClaraMundi
{
  public class FormElement : MonoBehaviour, ISelectHandler, IDeselectHandler
  {
    [HideInInspector]
    public FormElement AutoFocusElement;
    [HideInInspector]
    public FormElement PreviousElement;
    [HideInInspector]
    public FormElement NextElement;
    private bool lastActivated;
    public bool CanSubmit;
    public event Action SubmitAction;
    public TMP_InputField InputField;

    private bool nextPressed;
    private bool previousPressed;

    private float cooldown;

    private void OnEnable()
    {
      InputField ??= GetComponent<TMP_InputField>();
      if (AutoFocusElement == gameObject)
        Activate();

    }
    public void OnSelect(BaseEventData eventData)
    {
      Debug.Log("On Select!");
      if (InputManager.Instance == null) return;

      InputField?.ActivateInputField();
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
    }

    public void OnDeselect(BaseEventData eventData)
    {
      if (InputManager.Instance == null) return;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
    }


    private void OnDestroy()
    {
      SubmitAction = null;
    }

    private void OnNext(InputAction.CallbackContext context)
    {
      if (!IsActivated()) return;
      if (cooldown > 0) return;
      nextPressed = true;
    }
    private void OnPrevious(InputAction.CallbackContext context)
    {
      if (!IsActivated()) return;
      if (cooldown > 0) return;
      previousPressed = true;
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
      if (!CanSubmit) return;
      var input = InputField;
      if (input != null && input.isFocused)
      {
        SubmitAction?.Invoke();
        AutoFocusElement?.Activate();
      }
      else if (input == null)
      {
        SubmitAction?.Invoke();
        AutoFocusElement?.Activate();
      }
    }
    public void Activate()
    {
      previousPressed = false;
      nextPressed = false;
      Debug.Log(gameObject.name);
      EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void ActivateNext()
    {
      nextPressed = false;
      previousPressed = false;
      if (NextElement == null) return;
      if (NextElement.gameObject.activeInHierarchy)
        NextElement.Activate();
      else
        NextElement.ActivateNext();
    }

    public void ActivatePrevious()
    {
      previousPressed = false;
      nextPressed = false;
      if (PreviousElement == null) return;
      if (PreviousElement.gameObject.activeInHierarchy)
      {
        PreviousElement.Activate();
      }
      else
      {
        PreviousElement.ActivatePrevious();
      }
    }

    private void LateUpdate()
    {
      if (cooldown > 0)
        cooldown -= Time.deltaTime;
      else
      {
        if (previousPressed && IsActivated())
          ActivatePrevious();
        else if (nextPressed && IsActivated())
          ActivateNext();
      }
    }

    private bool IsActivated()
    {
      return EventSystem.current.currentSelectedGameObject == gameObject;
    }
  }
}