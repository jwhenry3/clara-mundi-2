using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class MoveToFront : MonoBehaviour, IPointerDownHandler
    {
        public Transform MovingObject;

        public Transform Parent;

        public void Awake()
        {
            Parent = transform.parent;
        }

        public void OnPointerDown(PointerEventData eventData) => Move();
        public void Move()
        {
            if (MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1) return;
            MovingObject.SetAsLastSibling();
            SelectFirstInteractable();
        }

        public void SelectFirstInteractable()
        {
            var firstInteractable = GetComponentInChildren<InteractableOnlyWhenFocused>();
            if (firstInteractable != null)
                EventSystem.current.SetSelectedGameObject(firstInteractable.gameObject);
        }

        public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;

        private void OnDisable()
        {
            MovingObject.SetAsFirstSibling();
        }
    }
}