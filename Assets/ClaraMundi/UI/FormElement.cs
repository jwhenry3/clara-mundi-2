﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ClaraMundi
{
  public class FormElement : MonoBehaviour, ISelectHandler, IDeselectHandler
  {
    public string ID;

    [HideInInspector]
    public FormElement PreviousElement;
    [HideInInspector]
    public FormElement NextElement;

    [HideInInspector]
    public Form Form;
    public bool CanSubmit;
    public bool CanCancel = true;
    public event Action SubmitAction;
    public TMP_InputField InputField;

    public bool QueuedForDestroy;

    private bool nextPressed;
    private bool previousPressed;

    private float cooldown;
    private bool listening;

    private ScrollRect scroller;
    private void Start()
    {
      ID = StringUtils.UniqueId();
      scroller = GetComponentInParent<ScrollRect>();
    }
    private void OnEnable()
    {
      InputField ??= GetComponent<TMP_InputField>();

    }
    public void SnapTo(Transform child)
    {
      if (scroller == null) return;
      Canvas.ForceUpdateCanvases();
      RectTransform rect = child as RectTransform;
      var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
      var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
      var endPos = contentPos - childPos;
      // If no horizontal scroll, then don't change contentPos.x
      endPos.x = 0;
      endPos.y -= rect.sizeDelta.y;
      // If no vertical scroll, then don't change contentPos.y
      if (!scroller.vertical) endPos.y = contentPos.y;
      scroller.content.anchoredPosition = endPos;
    }
    public void OnSelect(BaseEventData eventData)
    {
      // Debug.Log(gameObject.name + ": Select");
      if (InputManager.Instance == null) return;

      InputField?.ActivateInputField();
      Form.FocusedElement = this;
      listening = true;
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
      SnapTo(transform);
    }

    public void OnDeselect(BaseEventData eventData)
    {
      if (QueuedForDestroy)
        return;
      // Debug.Log(gameObject.name + ": Deselect");
      if (InputManager.Instance == null) return;
      listening = false;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      if (Form.FocusedElement == this)
        Form.FocusedElement = null;
    }

    void OnDisable()
    {
      if (listening)
      {
        listening = false;
        InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
        InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
        InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
        InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (Form != null && CanCancel)
      {
        Form.ElementCancel(this);
      }
    }
    private void OnDestroy()
    {
      QueuedForDestroy = true;
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
      }
      else if (input == null)
      {
        SubmitAction?.Invoke();
      }
    }
    public void Activate()
    {
      previousPressed = false;
      nextPressed = false;
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
        var activated = IsActivated();
        if (previousPressed && activated)
          ActivatePrevious();
        else if (nextPressed && activated)
          ActivateNext();
      }
    }

    private bool IsActivated()
    {
      return EventSystem.current.currentSelectedGameObject == gameObject;
    }
  }
}