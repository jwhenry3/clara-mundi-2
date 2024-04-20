
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class InputFieldWithHybridNav : TMP_InputField, ISelectHandler, IDeselectHandler
  {
    public static InputFieldWithHybridNav LastInput;
    private CanvasGroupFocus focus;
    protected override void Start()
    {
      base.Start();
      focus = GetComponentInParent<CanvasGroupFocus>();
      if (focus == null)
        Debug.Log(gameObject.name + " could not find focus object");
    }
    public override void OnSelect(BaseEventData eventData)
    {
      base.OnSelect(eventData);
      LastInput = this;
      ButtonWithHybridNav.LastButton = null;
      if (focus != null)
      {
        focus.LastFocused = null;
        focus.LastFocusInput = this;
      }
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      if (LastInput == this)
        LastInput = null;
      if (focus != null)
      {
        if (focus.LastFocusInput == this)
          focus.LastFocusInput = null;
      }
    }
  }
}