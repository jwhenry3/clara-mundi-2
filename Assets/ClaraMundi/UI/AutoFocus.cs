using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class AutoFocus : MonoBehaviour
  {
    private bool focused;
    private void LateUpdate()
    {
      if (!focused)
      {
        focused = true;
        Debug.Log(gameObject.name);
        EventSystem.current.SetSelectedGameObject(gameObject);
      }
    }

    private void OnDisable()
    {
      focused = false;
    }
  }
}