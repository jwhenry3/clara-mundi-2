using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class DisableUIOnClick : MonoBehaviour, IPointerDownHandler
  {
    public void OnPointerDown(PointerEventData eventData)
    {
      // Debug.Log("Disable UI");
      InputManager.Instance.World.Enable();
      InputManager.Instance.UI.Disable();
    }
  }
}