using System;
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
    public FormElement AutoFocusElement;
    public UnityEvent Submit;
    [HideInInspector]
    public FormElement FirstElement;
    [HideInInspector]
    public FormElement LastElement;

    public Action CancelPressed;
    public Form ParentForm;

    public bool IsOnlyUI;

    public List<FormElement> Elements;

    private bool nextPressed;
    private bool previousPressed;
    private float cooldown;


    private void Start()
    {
      Submit ??= new UnityEvent();
      InitializeElements();
    }
    public void InitializeElements(GameObject gameObject, int nestingLevel = 4, int currentLevel = 0)
    {
      if (currentLevel > nestingLevel) return;
      foreach (RectTransform child in gameObject.transform)
      {
        if (!child.gameObject.activeInHierarchy) continue;
        var element = child.GetComponent<FormElement>();
        if (element != null)
          Elements.Add(element);
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
        current.AutoFocusElement = AutoFocusElement;
        current.SubmitAction += () => Submit?.Invoke();
      }

      if (IsOnlyUI && selectFirst)
      {
        FirstElement.Activate();
      }
    }


    public void OnSelect(BaseEventData eventData)
    {
      if (!IsOnlyUI) return;
      if (InputManager.Instance == null) return;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
      InputManager.Instance.World.FindAction("Look").Disable();
      InputManager.Instance.World.FindAction("Move").Disable();
    }

    public void OnDeselect(BaseEventData eventData)
    {
      if (!IsOnlyUI) return;
      if (InputManager.Instance == null) return;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      if (EventSystem.current.currentSelectedGameObject == null)
      {
        InputManager.Instance.World.FindAction("Look").Enable();
        InputManager.Instance.World.FindAction("Move").Enable();
      }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (ParentForm != null)
        EventSystem.current.SetSelectedGameObject(ParentForm.gameObject);
      else
        EventSystem.current.SetSelectedGameObject(null);
      CancelPressed?.Invoke();
    }

    void OnEnable()
    {
      if (AutoFocusElement != null)
        AutoFocusElement.Activate();
      else if (IsOnlyUI)
      {
        EventSystem.current.SetSelectedGameObject(gameObject);
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