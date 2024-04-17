using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class Form : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
  {
    public UnityEvent Submit;
    [HideInInspector]
    public FormElement FirstElement;
    [HideInInspector]
    public FormElement LastElement;

    public UnityEvent CancelPressed;
    public Form ParentForm;

    private bool nextPressed;
    private bool previousPressed;
    private float cooldown;

    public FormElement PreviouslySelected;

    public FormElement FocusedElement;

    private FormElement cachedAutoFocus;
    private float autoFocusCacheTimer;

    public FormElement AutoFocusElement
    {
      get
      {
        if (autoFocusCacheTimer > 0)
          return cachedAutoFocus;
        autoFocusCacheTimer = 0.5f; // cache the autofocus element on initial access of the value
        return cachedAutoFocus = GetAutoFocus();
      }
    }

    private bool listening;

    public bool CanCancel;
    public bool PropagateCancel;

    private bool noAutoSelect;


    private void Start()
    {
      Submit ??= new UnityEvent();
      InitializeElements();
    }

    public FormElement GetAutoFocus()
    {
      AutoFocus elements = GetComponentInChildren<AutoFocus>();

      return elements?.GetComponent<FormElement>();
    }
    public void InitializeElements()
    {
      bool previousExists = false;
      FirstElement = null;
      LastElement = null;
      FormElement[] elements = GetComponentsInChildren<FormElement>();

      if (elements.Length == 0)
      {
        PreviouslySelected = null;
        return;
      }

      for (int i = 0; i < elements.Length; i++)
      {
        var current = elements[i];
        var last = i > 0 ? elements[i - 1] : elements[^1];
        var next = i < elements.Length - 1 ? elements[i + 1] : elements[0];
        if (last != current)
          current.PreviousElement = last;
        if (next != current)
          current.NextElement = next;
        current.Form = this;
        current.SubmitAction += () => Submit?.Invoke();
        if (current == PreviouslySelected)
          previousExists = true;
      }

      FirstElement = elements[0];
      LastElement = elements[^1];

      if (!previousExists)
        PreviouslySelected = null;
    }

    public void PropagateFocus(FormElement value)
    {
      FocusedElement = value;
      ParentForm?.PropagateFocus(value);
    }
    public void OnSelect(BaseEventData eventData)
    {
      // Debug.Log(gameObject.name + ": Select");
      PropagateFocus(null);
      if (InputManager.Instance == null) return;
      listening = true;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
      if (AutoFocusElement != null && !noAutoSelect)
      {
        if (gameObject.activeInHierarchy)
          StartCoroutine(Select(AutoFocusElement.gameObject));
        return;
      }
      noAutoSelect = false;
    }

    IEnumerator Select(GameObject gameObject)
    {
      yield return new WaitForSeconds(0.1f);
      EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
      // Debug.Log(gameObject.name + ": Deselect");
      if (InputManager.Instance == null) return;
      listening = false;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
    }

    void OnDisable()
    {
      if (listening)
      {
        listening = false;
        InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
        InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
        InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      }
    }

    public void ElementCancel(FormElement element)
    {
      PropagateFocus(null);
      if (PropagateCancel)
      {
        PreviouslySelected = null;
        if (CanCancel)
        {
          if (CancelPressed.GetPersistentEventCount() > 0)
          {
            CancelPressed.Invoke();
          }
          else
          {
            if (gameObject.activeInHierarchy)
              StartCoroutine(Select(ParentForm?.gameObject));
          }
        }
        return;
      }
      noAutoSelect = true;
      PreviouslySelected = null;
      if (gameObject.activeInHierarchy)
        StartCoroutine(Select(gameObject));
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (CanCancel)
      {
        if (CancelPressed.GetPersistentEventCount() > 0)
        {
          CancelPressed.Invoke();
        }
        else
        {
          if (gameObject.activeInHierarchy)
            StartCoroutine(Select(ParentForm?.gameObject ?? gameObject));
        }
      }
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

    private void LateUpdate()
    {
      if (autoFocusCacheTimer > 0)
        autoFocusCacheTimer -= Time.deltaTime;
      if (cooldown > 0)
        cooldown -= Time.deltaTime;
      else
      {
        if (!IsActivated()) return;
        if (previousPressed)
        {
          LastElement?.Activate();
          previousPressed = false;
          nextPressed = false;
          return;
        }
        else if (nextPressed)
        {
          FirstElement?.Activate();
          previousPressed = false;
          nextPressed = false;
        }
      }
    }

    private bool IsActivated()
    {
      return EventSystem.current.currentSelectedGameObject == gameObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      EventSystem.current.SetSelectedGameObject(gameObject);
    }
  }
}