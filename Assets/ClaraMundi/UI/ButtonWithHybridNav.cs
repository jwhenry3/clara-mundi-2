using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ButtonWithHybridNav : Button, ISelectHandler, IDeselectHandler
  {
    public static ButtonWithHybridNav LastButton;
    public static ButtonWithHybridNav CurrentButton;
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

    public Action onPointerEnter;
    public Action onPointerExit;
    public Action onSelect;
    public Action onDeselect;

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
      LastButton = this;
      CurrentButton = this;
      InputFieldWithHybridNav.LastInput = null;
      if (focus != null)
      {
        focus.LastFocusInput = null;
        focus.LastFocused = this;
      }
      SnapTo(transform);
      onSelect?.Invoke();
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

    public override void OnDeselect(BaseEventData eventData)
    {
      base.OnDeselect(eventData);
      CurrentButton = null;
      onDeselect?.Invoke();
    }
    protected override void OnDisable()
    {
      base.OnDisable();
      onDeselect?.Invoke();
      if (LastButton == this)
        LastButton = null;
      if (focus != null)
      {
        if (focus.LastFocused == this)
          focus.LastFocused = null;
      }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
      base.OnPointerEnter(eventData);
      onPointerEnter?.Invoke();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
      base.OnPointerExit(eventData);
      onPointerExit?.Invoke();
    }
  }
}