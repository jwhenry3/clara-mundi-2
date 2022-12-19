using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class AutoFocus : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}