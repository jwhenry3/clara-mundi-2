
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEditor.PackageManager;
using System;

namespace ClaraMundi
{
  public class InputFieldWithHybridNav : TMP_InputField, ISelectHandler, IDeselectHandler
  {
    public static InputFieldWithHybridNav LastInput;
    public static InputFieldWithHybridNav CurrentInput;
    [Header("Manual Navigation")]
    public Selectable upSelectable;
    public Selectable fallbackUpSelectable;

    public Selectable downSelectable;
    public Selectable fallbackDownSelectable;

    public Selectable leftSelectable;
    public Selectable fallbackLeftSelectable;

    public Selectable rightSelectable;
    public Selectable fallbackRightSelectable;
    private CanvasGroupFocus focus;
    private ScrollRect scroller;

    public event Action SubmitAction;

    private bool listening;

    protected override void Start()
    {
      base.Start();
      scroller = GetComponentInParent<ScrollRect>();
      focus = GetComponentInParent<CanvasGroupFocus>();
      if (focus == null)
        Debug.Log(gameObject.name + " could not find focus object");
    }
    private Selectable GetActive(Selectable selectable)
    {
      if (selectable != null && selectable.gameObject.activeInHierarchy)
        return selectable;
      return null;
    }

    public override Selectable FindSelectableOnUp()
    {
      return GetActive(upSelectable) ?? GetActive(fallbackUpSelectable) ?? base.FindSelectableOnUp();
    }

    public override Selectable FindSelectableOnDown()
    {
      return GetActive(downSelectable) ?? GetActive(fallbackDownSelectable) ?? base.FindSelectableOnDown();
    }

    public override Selectable FindSelectableOnLeft()
    {
      return GetActive(leftSelectable) ?? GetActive(fallbackLeftSelectable) ?? base.FindSelectableOnLeft();
    }

    public override Selectable FindSelectableOnRight()
    {
      return GetActive(rightSelectable) ?? GetActive(fallbackRightSelectable) ?? base.FindSelectableOnRight();
    }

    public override void OnSelect(BaseEventData eventData)
    {
      base.OnSelect(eventData);
      LastInput = this;
      CurrentInput = this;
      ButtonWithHybridNav.LastButton = null;
      if (focus != null)
      {
        focus.LastFocused = null;
        focus.LastFocusInput = this;
        focus.InputActionAsset.FindAction("UI/NextElement").performed += OnNext;
        focus.InputActionAsset.FindAction("UI/PreviousElement").performed += OnPrevious;
        focus.InputActionAsset.FindAction("UI/Submit").performed += OnSubmit;
        listening = true;
      }
      SnapTo(transform);
      ActivateInputField();
    }

    void OnNext(InputAction.CallbackContext context)
    {
      var next = FindSelectableOnRight() ?? FindSelectableOnDown();
      Debug.Log("Next Element Please");
      Debug.Log(next);
      if (next != null)
        EventSystem.current.SetSelectedGameObject(next.gameObject);
    }

    void OnPrevious(InputAction.CallbackContext context)
    {
      var previous = FindSelectableOnLeft() ?? FindSelectableOnUp();
      if (previous != null)
        EventSystem.current.SetSelectedGameObject(previous.gameObject);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
      base.OnDeselect(eventData);
      CurrentInput = null;
      if (focus != null)
      {
        focus.InputActionAsset.FindAction("UI/NextElement").performed -= OnNext;
        focus.InputActionAsset.FindAction("UI/PreviousElement").performed -= OnPrevious;
        focus.InputActionAsset.FindAction("UI/Submit").performed -= OnSubmit;
        listening = false;
      }
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
    protected override void OnDisable()
    {
      base.OnDisable();
      if (LastInput == this)
        LastInput = null;
      if (focus != null)
      {
        if (listening)
        {
          focus.InputActionAsset.FindAction("UI/NextElement").performed -= OnNext;
          focus.InputActionAsset.FindAction("UI/PreviousElement").performed -= OnPrevious;
          focus.InputActionAsset.FindAction("UI/Submit").performed -= OnSubmit;
        }
        if (focus.LastFocusInput == this)
          focus.LastFocusInput = null;
      }
    }


    private void OnSubmit(InputAction.CallbackContext context)
    {
      SubmitAction?.Invoke();
    }
  }
}