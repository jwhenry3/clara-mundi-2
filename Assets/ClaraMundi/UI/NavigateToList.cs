using System;
using System.Collections;
using System.Collections.Generic;
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
    public InputActionAsset InputActionAsset;
    private InputAction InputAction;

    public GameObject List;
    public bool IsNestedList;

    public NavigateToListMap[] Mapping;
    private bool listening;
    void Awake()
    {
      InputAction = InputActionAsset.FindAction("UI/Navigate");
    }

    public void OnSelect(BaseEventData eventData)
    {
      InputAction.performed += OnPerform;
      listening = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
      InputAction.performed -= OnPerform;
      listening = false;
    }

    public void OnDisable()
    {
      if (listening)
        InputAction.performed -= OnPerform;
      listening = false;
    }

    private void OnPerform(InputAction.CallbackContext context)
    {
      if (!gameObject.activeInHierarchy) return;
      Vector2 value = context.ReadValue<Vector2>();
      foreach (var mapping in Mapping)
      {
        switch (mapping.Direction)
        {
          case Direction.Up when value.y > 0:
          case Direction.Down when value.y < 0:
          case Direction.Left when value.x < 0:
          case Direction.Right when value.x > 0:
            StartCoroutine(Select(mapping));
            break;
        }
      }
    }

    GameObject GetChildForMapping(NavigateToListMap mapping, GameObject list, bool hasNested = false)
    {
      if (list.transform.childCount == 0) return null;

      if (IsNestedList && !hasNested)
      {
        foreach (Transform child in list.transform)
        {
          if (child.gameObject.activeInHierarchy)
            return GetChildForMapping(mapping, child.gameObject, true);
        }
        return null;
      }
      Debug.Log("Find Element");
      Debug.Log("List " + list);
      if (mapping.Element == ElementOfList.Last)
      {
        Transform lastChild = null;
        foreach (Transform child in list.transform)
        {
          if (child.gameObject.activeInHierarchy)
            lastChild = child;
        }
        return lastChild?.gameObject;
      }
      foreach (Transform child in list.transform)
      {
        if (child.gameObject.activeInHierarchy)
          return child.gameObject;
      }
      Debug.Log("NOT FOUND");
      return null;
    }

    GameObject GetSelectable(GameObject obj, int nestingLevel = 0)
    {
      if (nestingLevel > 3 || obj == null)
        return null;
      if (obj.GetComponent<ButtonWithHybridNav>() || obj.GetComponent<InputFieldWithHybridNav>())
        return obj;
      return GetSelectable(obj.transform.GetChild(0).gameObject);
    }

    IEnumerator Select(NavigateToListMap mapping)
    {
      if (!gameObject.activeInHierarchy) yield return null;
      yield return new WaitForSeconds(0.1f);
      GameObject child = GetSelectable(GetChildForMapping(mapping, List));
      Debug.Log(child);
      if (child != null)
        EventSystem.current.SetSelectedGameObject(child);
    }

  }
}