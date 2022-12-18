using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class MoveToFront : MonoBehaviour, IPointerDownHandler
    {
        public Transform MovingObject;

        public void OnPointerDown(PointerEventData eventData)
        {
            MovingObject.SetAsLastSibling();
        }
    }
}