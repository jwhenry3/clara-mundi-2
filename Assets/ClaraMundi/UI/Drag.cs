using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rect;
        private RectTransform canvasRect;

        public int GridUnit = 16;

        private Vector2 pointerOffset;
        private bool isDragging;
        private Transform target;
        
        private Vector2 lastMousePosition;
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            lastMousePosition = eventData.position;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out pointerOffset);
        }
        public void OnDrag(PointerEventData eventData)
        {

            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;

            var position = rect.position;
            Vector3 newPosition = position +  new Vector3(diff.x, diff.y, 0);
            position = newPosition;
            rect.position = position;
            lastMousePosition = currentMousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            var pos = rect.position;
            pos = new Vector3(
                Mathf.Round(pos.x / GridUnit) * GridUnit,
                Mathf.Round(pos.y / GridUnit) * GridUnit,
                1
            );
            rect.position = pos;
        }
    }
}