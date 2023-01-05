using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class MoveToFront : HoverDisablesWorldInput, IPointerDownHandler
    {
        public Transform MovingObject;

        Transform Parent;
        Transform GrandParent;
        private InteractableOnlyWhenFocused firstInteractable;

        public void Awake()
        {
            Parent = transform.parent;
            GrandParent = Parent.parent;
        }

        public void OnPointerDown(PointerEventData eventData) => Move();
        public void Move()
        {
            if (MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1) return;
            MovingObject.SetAsLastSibling();
            SelectFirstInteractable();
            firstInteractable = GetComponentInChildren<InteractableOnlyWhenFocused>();
            GameWindowHandler.Instance.ActiveWindow = this;
        }

        public void SelectFirstInteractable()
        {
            if (firstInteractable != null)
                EventSystem.current.SetSelectedGameObject(firstInteractable.gameObject);
        }

        public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;

        private void OnDisable()
        {
            if (GameWindowHandler.Instance.ActiveWindow == this)
                GameWindowHandler.Instance.ActiveWindow = null;
            if (!gameObject.activeInHierarchy) return;
            MovingObject.SetAsFirstSibling();
        }

        private float updateTick;
        private void Update()
        {
            updateTick += Time.deltaTime;
            if (GameWindowHandler.Instance.ActiveWindow == null)
            {
                if (Parent.GetSiblingIndex() ==GrandParent.childCount - 1)
                    GameWindowHandler.Instance.ActiveWindow = this;
            }
            if (!(updateTick > 5)) return;
            updateTick = 0;
            firstInteractable = GetComponentInChildren<InteractableOnlyWhenFocused>();
            
        }
    }
}