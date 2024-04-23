
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

    private TabNavigation tabNav;

    protected override void Start()
    {
      base.Start();
      scroller = GetComponentInParent<ScrollRect>();
      focus = GetComponentInParent<CanvasGroupFocus>();
      tabNav = GetComponent<TabNavigation>() ?? gameObject.AddComponent<TabNavigation>();
      tabNav.selectable = this;
      tabNav.focus = focus;
      if (focus == null)
        Debug.Log(gameObject.name + " could not find focus object");
      onValidateInput += delegate (string input, int charIndex, char addedChar) { return DisableTabValidate(addedChar); };
    }
    private char DisableTabValidate(char charToValidate)
    {
      //Checks if a tab sign is entered....
      if (charToValidate == '\t')
      {
        // ... if it is change it to an empty character.
        charToValidate = '\0';
      }
      return charToValidate;
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

    void Update()
    {
      tabNav?.Update();
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
      SubmitAction?.Invoke();
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
        tabNav?.Listen(OnSubmit);
      }
      SnapTo(transform);
      ActivateInputField();
    }


    public override void OnDeselect(BaseEventData eventData)
    {
      base.OnDeselect(eventData);
      CurrentInput = null;
      if (focus != null)
      {
        tabNav?.StopListening(OnSubmit);
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
        tabNav?.StopListening();
        if (focus.LastFocusInput == this)
          focus.LastFocusInput = null;
      }
    }
  }
}