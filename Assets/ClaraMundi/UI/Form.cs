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

    public bool IsOnlyUI;

    public List<FormElement> Elements;

    private bool nextPressed;
    private bool previousPressed;
    private float cooldown;


    public FormElement SelfElement;


    public FormElement PreviouslySelected;

    public FormElement FocusedElement;
    public static FormElement Focused;

    public bool selectOnEnable;

    private bool listening;

    private CanvasGroup canvasGroup;


    private void Start()
    {
      Submit ??= new UnityEvent();
      InitializeElements();
      SelfElement = GetComponent<FormElement>();
      canvasGroup = GetComponent<CanvasGroup>();
    }


    public void InitializeElements(GameObject gameObject, int nestingLevel = 4, int currentLevel = 0)
    {
      if (currentLevel > nestingLevel) return;
      foreach (RectTransform child in gameObject.transform)
      {
        if (!child.gameObject.activeInHierarchy) continue;
        var element = child.GetComponent<FormElement>();
        var form = child.GetComponent<Form>();
        if (element != null)
          Elements.Add(element);
        else if (form != null) continue; // do not eat up child form elements
        else
          InitializeElements(child.gameObject, nestingLevel, currentLevel + 1);
      }
    }
    public void InitializeElements(bool selectFirst = false, int nestingLevel = 4)
    {
      Elements = new();
      InitializeElements(gameObject, nestingLevel);
      if (Elements.Count == 0)
      {
        FirstElement = null;
        LastElement = null;
        return;
      }
      FirstElement = Elements.First();
      LastElement = Elements.Last();
      bool previousExists = false;
      for (int i = 0; i < Elements.Count; i++)
      {
        var current = Elements[i];
        var last = i > 0 ? Elements[i - 1] : Elements[^1];
        var next = i < Elements.Count - 1 ? Elements[i + 1] : Elements[0];
        if (last != current)
          current.PreviousElement = last;
        if (next != current)
          current.NextElement = next;
        current.Form = this;
        current.SubmitAction += () => Submit?.Invoke();
        if (current == PreviouslySelected)
          previousExists = true;
      }
      if (!previousExists)
        PreviouslySelected = null;
    }

    public void PropagateFocus(FormElement value)
    {
      FocusedElement = value;
      Focused = value;
      ParentForm?.PropagateFocus(value);
    }
    public void OnSelect(BaseEventData eventData)
    {
      if (InputManager.Instance == null) return;
      listening = true;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;

      if (PreviouslySelected != null)
      {
        PreviouslySelected.Activate();
        return;
      }
      if (FirstElement != null)
      {
        FirstElement.Activate();
      }
    }

    public void OnDeselect(BaseEventData eventData)
    {
      if (InputManager.Instance == null) return;
      listening = false;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (ParentForm != null)
        EventSystem.current.SetSelectedGameObject(ParentForm.gameObject);
      else
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ElementCanceled()
    {
      if (CancelPressed.GetPersistentEventCount() > 0)
        CancelPressed?.Invoke();
      else
        EventSystem.current.SetSelectedGameObject(gameObject);
    }


    void OnEnable()
    {
      // if (IsOnlyUI || selectOnEnable)
      // {
      //   EventSystem.current.SetSelectedGameObject(gameObject);
      // }
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