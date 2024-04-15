using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class HoverDisablesInputs : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public static List<HoverDisablesInputs> Hovered = new();
    public bool ShouldDisableWorldInput;
    public List<string> InputsToDisable;
    public void OnPointerEnter(PointerEventData eventData)
    {
      if (!Hovered.Contains(this))
        Hovered.Add(this);
      if (Hovered.Count > 1) return; // already handled by another hover component
      if (ShouldDisableWorldInput)
        InputManager.Instance.World.Disable();
      if (InputsToDisable?.Count > 0)
      {
        InputsToDisable.ForEach(input =>
        {
          var action = InputManager.Instance.World.FindAction(input);
          if (action != null)
          {
            action.Disable();
          }
        });
      }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      OnDisable();
    }
    protected virtual void OnDisable()
    {
      if (Hovered.Contains(this))
        Hovered.Remove(this);
      if (Hovered.Count > 0) return; // already handled by another hover component
      if (InputsToDisable?.Count > 0)
      {
        InputsToDisable.ForEach(input =>
        {
          var action = InputManager.Instance.World.FindAction(input);
          if (action != null)
          {
            action.Enable();
          }
        });
      }
    }
  }
}