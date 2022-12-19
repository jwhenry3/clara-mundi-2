using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class FocusFirstChild : MonoBehaviour
    {
        private void Update()
        {
            if (transform.childCount == 0) return;
            if (EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(transform.GetChild(0).gameObject);
        }
    }
}