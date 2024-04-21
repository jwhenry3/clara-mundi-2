using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace ClaraMundi
{
  [Serializable]
  public class ControlsDictionary : UnitySerializedDictionary<string, CanvasGroupFocus> { }
  public class TopLevelCanvas : MonoBehaviour
  {

    public static TopLevelCanvas Instance;
    public InputActionAsset InputActionAsset;

    public ControlsDictionary Controls;

    void OnEnable()
    {
      Instance = this;
      InputActionAsset.FindAction("UI/Navigate").performed += OnSelect;
      InputActionAsset.FindAction("UI/NextElement").performed += OnSelect;
    }

    void OnDisable()
    {
      InputActionAsset.FindAction("UI/Navigate").performed -= OnSelect;
      InputActionAsset.FindAction("UI/NextElement").performed -= OnSelect;
    }
    public void OnSelect(InputAction.CallbackContext eventData)
    {
      if (EventSystem.current.currentSelectedGameObject != null) return;
      if (ButtonWithHybridNav.LastButton != null)
        EventSystem.current.SetSelectedGameObject(ButtonWithHybridNav.LastButton.gameObject);
      else if (InputFieldWithHybridNav.LastInput != null)
        EventSystem.current.SetSelectedGameObject(InputFieldWithHybridNav.LastInput.gameObject);
      else
      {
        // find first active UI section and select the first relevant element if possible
        foreach (CanvasGroupFocus control in Controls.Values)
        {
          if (control.gameObject.activeInHierarchy && control.canvasGroup.interactable)
          {
            control.Select();
            return;
          }
        }
      }
    }
  }
}