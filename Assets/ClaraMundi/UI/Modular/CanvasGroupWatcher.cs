using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class CanvasGroupWatcher : MonoBehaviour, IPointerDownHandler
  {
    public CanvasGroup group;
    public WindowUI window;

    public ButtonUI CurrentButton;
    public InputUI CurrentInput;
    public DropdownUI CurrentDropdown;
    public ButtonUI AutoFocusButton;
    public InputUI AutoFocusInput;
    public DropdownUI AutoFocusDropdown;

    private bool lastInteractable = false;

    void Update()
    {
      if (BecameInteractable())
      {
        Debug.Log(gameObject.name + " Interactable!");
        if (CurrentButton != null && CurrentButton.button.IsInteractable())
          CurrentButton.Select();
        else if (CurrentInput != null && CurrentInput.inputField.IsInteractable())
          CurrentInput.Select();
        else if (CurrentDropdown != null && CurrentDropdown.dropdown.IsInteractable())
          CurrentDropdown.Select();
        else if (AutoFocusButton != null && AutoFocusButton.button.IsInteractable())
          AutoFocusButton.Select();
        else if (AutoFocusInput != null && AutoFocusInput.inputField.IsInteractable())
          AutoFocusInput.Select();
        else if (AutoFocusDropdown != null && AutoFocusDropdown.dropdown.IsInteractable())
          AutoFocusDropdown.Select();
        else
        {
          lastInteractable = false;
          return;
        }
      }
      else if (lastInteractable && !IsInteractable())
      {
        Debug.Log(gameObject.name + " Not Interactable!");
      }
      lastInteractable = IsInteractable();
    }

    void OnDisable()
    {
      lastInteractable = false;
    }

    public bool IsInteractable()
    {
      return group.isActiveAndEnabled && group.interactable;
    }

    bool BecameInteractable()
    {
      var result = IsInteractable() != lastInteractable;
      if (result)
        return IsInteractable();
      return false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (CurrentButton != null && CurrentButton.button.IsInteractable())
        CurrentButton.Select();
      else if (CurrentInput != null && CurrentInput.inputField.IsInteractable())
        CurrentInput.Select();
      else if (CurrentDropdown != null && CurrentDropdown.dropdown.IsInteractable())
        CurrentDropdown.Select();
      else if (AutoFocusButton != null && AutoFocusButton.button.IsInteractable())
        AutoFocusButton.Select();
      else if (AutoFocusInput != null && AutoFocusInput.inputField.IsInteractable())
        AutoFocusInput.Select();
      else if (AutoFocusDropdown != null && AutoFocusDropdown.dropdown.IsInteractable())
        AutoFocusDropdown.Select();
    }
  }
}