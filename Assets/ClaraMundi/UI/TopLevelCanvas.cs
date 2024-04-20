using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class TopLevelCanvas : MonoBehaviour
  {
    public InputActionAsset InputActionAsset;

    void OnEnable()
    {
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
    }
  }
}