using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class CanvasGroupWatcher : UIBehaviour, IPointerDownHandler
  {
    public CanvasGroup group;
    public bool RecordPressed = true;

    public ButtonUI CurrentButton;
    public InputUI CurrentInput;
    public DropdownUI CurrentDropdown;
    public ButtonUI AutoFocusButton;
    public InputUI AutoFocusInput;
    public DropdownUI AutoFocusDropdown;

    private bool lastInteractable = false;
    public void SetCurrentButton(ButtonUI button)
    {
      CurrentButton = button;
    }

    private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
    private bool m_GroupsAllowInteraction = false;
    protected override void OnCanvasGroupChanged()
    {
      var parentGroupAllowsInteraction = ParentGroupAllowsInteraction();

      if (parentGroupAllowsInteraction != m_GroupsAllowInteraction)
      {
        m_GroupsAllowInteraction = parentGroupAllowsInteraction;
      }
    }
    protected override void OnEnable()
    {
      m_GroupsAllowInteraction = ParentGroupAllowsInteraction();
    }

    bool ParentGroupAllowsInteraction()
    {
      Transform t = transform;
      while (t != null)
      {
        t.GetComponents(m_CanvasGroupCache);
        for (var i = 0; i < m_CanvasGroupCache.Count; i++)
        {
          if (m_CanvasGroupCache[i].enabled && !m_CanvasGroupCache[i].interactable)
            return false;

          if (m_CanvasGroupCache[i].ignoreParentGroups)
            return true;
        }

        t = t.parent;
      }

      return true;
    }
    void LateUpdate()
    {
      if (!lastInteractable && group.interactable && m_GroupsAllowInteraction)
      {
        if (!FocusButton(CurrentButton) && !FocusInput(CurrentInput) && !FocusDropdown(CurrentDropdown))
          if (!FocusButton(AutoFocusButton) && !FocusInput(AutoFocusInput) && !FocusDropdown(AutoFocusDropdown))
          {
            lastInteractable = false;
            return;
          }
      }
      lastInteractable = group.interactable && m_GroupsAllowInteraction;
    }

    bool FocusButton(ButtonUI button)
    {
      if (button != null)
      {
        if (button.button != null && button.gameObject.activeInHierarchy && button.button.IsInteractable())
        {
          button.Select();
          return true;
        }
      }
      return false;
    }
    bool FocusDropdown(DropdownUI dropdown)
    {
      if (dropdown != null)
      {
        if (dropdown.dropdown != null && dropdown.gameObject.activeInHierarchy && dropdown.dropdown.IsInteractable())
        {
          dropdown.Select();
          return true;
        }
      }
      return false;
    }
    bool FocusInput(InputUI input)
    {
      if (input != null)
      {
        if (input.inputField != null && input.gameObject.activeInHierarchy && input.inputField.IsInteractable())
        {
          input.Select();
          return true;
        }
      }
      return false;
    }

    protected override void OnDisable()
    {
      lastInteractable = false;
    }

    public bool IsInteractable()
    {
      return group.isActiveAndEnabled && group.interactable;
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