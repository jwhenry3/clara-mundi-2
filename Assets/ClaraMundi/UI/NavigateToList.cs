using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public enum Direction
  {
    Up,
    Down,
    Left,
    Right
  }
  public enum ElementOfList
  {
    First,
    Last,
  }
  [Serializable]
  public struct NavigateToListMap
  {
    public Direction Direction;
    public ElementOfList Element;
  }
  public class NavigateToList : MonoBehaviour, ISelectHandler, IDeselectHandler
  {
    private InputAction InputAction;

    public GameObject List;

    public NavigateToListMap[] Mapping;

    void Awake()
    {
      InputAction = InputManager.Instance.UI.FindAction("Navigate");
    }

    public void OnSelect(BaseEventData eventData)
    {
      InputAction.performed += OnPerform;
    }

    public void OnDeselect(BaseEventData eventData)
    {
      InputAction.performed -= OnPerform;
    }

    private void OnPerform(InputAction.CallbackContext context)
    {
      Vector2 value = context.ReadValue<Vector2>();
      foreach (var mapping in Mapping)
      {
        switch (mapping.Direction)
        {
          case Direction.Up when value.y > 0:
          case Direction.Down when value.y < 0:
          case Direction.Left when value.x < 0:
          case Direction.Right when value.x > 0:
            Select(mapping);
            break;
        }
      }
    }

    GameObject GetChildForMapping(NavigateToListMap mapping)
    {
      if (List.transform.childCount == 0) return null;
      if (mapping.Element == ElementOfList.Last)
        return List.transform.GetChild(List.transform.childCount - 1).gameObject;
      return List.transform.GetChild(0)?.gameObject;
    }

    private void Select(NavigateToListMap mapping)
    {
      GameObject child = GetChildForMapping(mapping);
      if (child != null)
        EventSystem.current.SetSelectedGameObject(child);
    }
  }
}