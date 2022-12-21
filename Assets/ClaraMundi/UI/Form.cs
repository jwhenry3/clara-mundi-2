using System;
using UnityEngine;
using UnityEngine.Events;

namespace ClaraMundi
{
    public class Form : MonoBehaviour
    {
        public GameObject AutoFocusElement;
        public UnityEvent Submit;
        private void Awake()
        {
            Submit ??= new UnityEvent();
            InitializeElements();
        }

        private void InitializeElements()
        {
            var elements = GetComponentsInChildren<FormElement>();
            for (int i = 0; i < elements.Length; i++)
            {
                var current = elements[i];
                var last = i > 0 ? elements[i - 1] : elements[^1];
                var next = i < elements.Length - 1 ? elements[i + 1] : elements[0];
                if (last != current)
                    current.PreviousElement = last.gameObject;
                if (next != current)
                    current.NextElement = next.gameObject;
                current.AutoFocusElement = AutoFocusElement;
                current.SubmitAction += () => Submit?.Invoke();
            }
        }
    }
}