using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class AutoFocus : MonoBehaviour
  {
    public bool HasFocused;
    private void LateUpdate()
    {
      if (!HasFocused)
      {
        HasFocused = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
      }
    }

    private void OnDisable()
    {
      HasFocused = false;
    }
  }
}