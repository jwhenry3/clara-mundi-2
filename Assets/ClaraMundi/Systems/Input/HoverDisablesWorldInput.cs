using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class HoverDisablesWorldInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            InputManager.Instance.World.Disable();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InputManager.Instance.World.Enable();
        }
    }
}