using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class DisableUIOnClick : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Disable UI");
            InputManager.Instance.World.Enable();
            InputManager.Instance.UI.Disable();
        }
    }
}