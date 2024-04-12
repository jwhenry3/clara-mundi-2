using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rect;
        private Transform t;

        public int GridUnit = 16;
        
        private Vector2 lastMousePosition;
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            t = GetComponent<Transform>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            lastMousePosition = eventData.position;
            Debug.Log("Hey!");
        }
        public void OnDrag(PointerEventData eventData)
        {

            Vector2 currentMousePosition = eventData.position;
            Vector2 diff = currentMousePosition - lastMousePosition;

            var position = GetPosition();
            Vector3 newPosition = position +  new Vector3(diff.x, diff.y, 0);
            position = newPosition;
            SetPosition(position.x, position.y, position.z);
            lastMousePosition = currentMousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var pos = GetPosition();
            pos = new Vector3(
                Mathf.Round(pos.x / GridUnit) * GridUnit,
                Mathf.Round(pos.y / GridUnit) * GridUnit,
                1
            );
            SetPosition(pos.x, pos.y, pos.z);
        }

        Vector3 GetPosition() {
          if (rect != null)
            return rect.position;
          return t.position;
        }
        void SetPosition(float x, float y, float z) {
          if (rect != null) {
            rect.position = new Vector3(x, y, z);
            return;
          }
          t.position = new Vector3(x, y, z);
        }

    }
}