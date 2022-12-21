using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
    public class FormElement : MonoBehaviour
    {
        [HideInInspector]
        public GameObject AutoFocusElement;
        [HideInInspector]
        public GameObject PreviousElement;
        [HideInInspector]
        public GameObject NextElement;
        private bool lastActivated;
        public event Action SubmitAction;


        private void OnActivated()
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
            InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
            InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
        }

        private void OnDeactivated()
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
            InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
            InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
        }

        private void OnDisable()
        {
            if (!lastActivated) return;
            OnDeactivated();
        }

        private void OnDestroy()
        {
            SubmitAction = null;
            if (!lastActivated) return;
            OnDeactivated();
        }
        private void OnNext(InputAction.CallbackContext context)
        {
            if (!IsActivated()) return;
            if (NextElement == null) return;
            Activate(NextElement);
        }
        private void OnPrevious(InputAction.CallbackContext context)
        {
            if (!IsActivated()) return;
            if (PreviousElement == null) return;
            // DeactivateInput();
            Activate(PreviousElement);
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            var input = GetComponent<TMP_InputField>();
            if (input != null && input.isFocused)
            {
                SubmitAction?.Invoke();
                if (AutoFocusElement != null)
                    Activate(AutoFocusElement);
            } 
            else if (input == null)
            {
                SubmitAction?.Invoke();
                if (AutoFocusElement != null)
                    Activate(AutoFocusElement);
            }
        }

        private void Activate(GameObject element)
        {
            var input = element.GetComponent<TMP_InputField>();
            if (input != null) 
                input.ActivateInputField();
            else
                EventSystem.current.SetSelectedGameObject(element);
        }

        private void Update()
        {
            var activated = IsActivated();
            if (lastActivated && !activated)
                OnDeactivated();
            if (!lastActivated && activated)
                OnActivated();
            lastActivated = activated;
        }

        private bool IsActivated()
        {
            return EventSystem.current.currentSelectedGameObject == gameObject;
        }
    }
}