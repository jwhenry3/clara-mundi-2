using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class MoveToFront : MonoBehaviour, IPointerDownHandler
    {
        public Transform MovingObject;

        public Transform Parent;
        private InteractableOnlyWhenFocused firstInteractable;

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
            firstInteractable = GetComponentInChildren<InteractableOnlyWhenFocused>();
            UIManager.Instance.ActiveWindow = this;
        }

        public void SelectFirstInteractable()
        {
            if (firstInteractable != null)
                EventSystem.current.SetSelectedGameObject(firstInteractable.gameObject);
        }

        public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;

        private void OnDisable()
        {
            if (UIManager.Instance.ActiveWindow == this)
                UIManager.Instance.ActiveWindow = null;
            MovingObject.SetAsFirstSibling();
        }

        private float updateTick;
        private void Update()
        {
            updateTick += Time.deltaTime;
            if (!(updateTick > 5)) return;
            updateTick = 0;
            firstInteractable = GetComponentInChildren<InteractableOnlyWhenFocused>();
        }
    }
}