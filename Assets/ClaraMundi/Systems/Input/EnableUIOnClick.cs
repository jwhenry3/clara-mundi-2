using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class EnableUIOnClick : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Enable UI");
            InputManager.Instance.World.Disable();
            InputManager.Instance.UI.Enable();
        }
    }
}