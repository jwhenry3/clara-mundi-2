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

        public void OnEndDrag(PointerEventData eventData)
        {
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