using System;
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

    [HideInInspector]
    public Form Form;
    private Form selfForm;
    public bool CanSubmit;
    public event Action SubmitAction;
    public TMP_InputField InputField;

    private bool nextPressed;
    private bool previousPressed;

    private float cooldown;

    private void OnEnable()
    {
      selfForm = GetComponent<Form>();
      InputField ??= GetComponent<TMP_InputField>();
      if (AutoFocusElement == gameObject)
        Activate();

    }
    public void OnSelect(BaseEventData eventData)
    {
      Debug.Log("On Select!");
      if (InputManager.Instance == null) return;

      if (Form != null && Form.FocusedElement != this)
      {
        Form.PreviouslySelected = this;
        Form.PropagateFocus(this);
      }
      InputField?.ActivateInputField();
      InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
      InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
      InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
    }

    public void OnDeselect(BaseEventData eventData)
    {
      if (InputManager.Instance == null) return;
      InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
      InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
      InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      if (Form != null && Form.FocusedElement == this)
        Form.PropagateFocus(null);
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (Form != null)
      {
        Form.PreviouslySelected = null;
        Form.ElementCanceled();
      }
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