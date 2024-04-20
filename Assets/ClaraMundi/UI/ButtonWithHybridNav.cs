using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ButtonWithHybridNav : Button, ISelectHandler, IDeselectHandler
  {
    public static ButtonWithHybridNav LastButton;
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

    protected override void Start()
    {
      base.Start();
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
      InputFieldWithHybridNav.LastInput = null;
      if (focus != null)
      {
        focus.LastFocusInput = null;
        focus.LastFocused = this;
      }
    }
    protected override void OnDisable()
    {
      base.OnDisable();
      if (LastButton == this)
        LastButton = null;
      if (focus != null)
      {
        if (focus.LastFocused == this)
          focus.LastFocused = null;
      }
    }
  }
}