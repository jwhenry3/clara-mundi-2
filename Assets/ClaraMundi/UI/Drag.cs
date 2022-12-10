using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private RectTransform rect;
        private RectTransform canvasRect;

        private Vector2 pointerOffset;
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out pointerOffset);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out var localPointerPosition))
                rect.localPosition = localPointerPosition - pointerOffset;
        }
    }
}