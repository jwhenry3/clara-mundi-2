using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
    public class InteractableOnlyWhenFocused : MonoBehaviour, IPointerClickHandler
    {
        
        private MoveToFront MoveToFront;
        private Selectable Selectable;
        private Button Button;
        private CanvasGroup CanvasGroup;
        private bool interactable;

        public bool IsFocused() {
          return interactable;
        }

        private void Awake()
        {
            MoveToFront = GetComponentInParent<MoveToFront>();
            CanvasGroup = GetComponent<CanvasGroup>();
            Selectable = GetComponent<Selectable>();
            Button = GetComponent<Button>();
        }

        private void Update()
        {
            if (MoveToFront == null) return;
            SetStatus(MoveToFront.IsInFront());
        }

        private void SetStatus(bool value)
        {
            if (CanvasGroup != null)
                CanvasGroup.interactable = value;
            if (Selectable != null)
                Selectable.interactable = value;
            else if (Button != null)
                Button.interactable = value;
            interactable = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                MoveToFront.Move();
        }
    }
}