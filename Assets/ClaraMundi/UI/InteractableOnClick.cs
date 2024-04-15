using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class InteractableOnClick : MonoBehaviour, IPointerDownHandler
  {
    public CanvasGroup CanvasGroup;

    public string ActionName;
    public InputAction InputAction;


    void Update()
    {
      if (InputManager.Instance != null && InputAction == null)
      {
        InputAction = InputManager.Instance.UI.FindAction(ActionName);
        InputAction.performed += OnPerform;
      }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      CanvasGroup.interactable = true;
    }

    void OnPerform(InputAction.CallbackContext context)
    {
      CanvasGroup.interactable = true;
    }
  }

}