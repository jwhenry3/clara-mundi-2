using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClaraMundi
{
  public class MoveSibling : MonoBehaviour, IPointerDownHandler
  {
    public Transform MovingObject;
    public CanvasGroup CanvasGroupToToggle;
    public CanvasGroup[] CanvasGroupsToToggle;
    public GameObject[] GameObjectsToToggle;

    public Action SentToBack;
    public Action SentToFront;

    public bool MoveObjectToBackOnClick;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
      if (!enabled) return;
      if (MoveObjectToBackOnClick)
        ToBack();
      else
      {
        ToFront();
        StartCoroutine(SelectObjectAtCursorPosition(eventData));
      }
    }

    IEnumerator SelectObjectAtCursorPosition(PointerEventData eventData)
    {
      yield return new WaitForSeconds(0.1f);
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, results);

      foreach (var result in results)
      {
        var button = result.gameObject.GetComponent<ButtonUI>();
        var input = result.gameObject.GetComponent<InputUI>();
        if (button != null)
          button.Select();
        if (input != null)
          input.Select();
      }
    }

    public void ToFront()
    {
      if (!enabled) return;
      if (!IsInFront())
        MovingObject.SetAsLastSibling();
      if (!MovingObject.gameObject.activeInHierarchy)
        MovingObject.gameObject.SetActive(true);
      ToggleGroups(true);
      SentToFront?.Invoke();
    }

    void ToggleGroups(bool value)
    {
      if (CanvasGroupToToggle != null)
      {
        CanvasGroupToToggle.interactable = value;
        CanvasGroupToToggle.blocksRaycasts = value;
      }
      foreach (var group in CanvasGroupsToToggle)
      {
        group.interactable = value;
        group.blocksRaycasts = value;
      }
      foreach (var obj in GameObjectsToToggle)
        obj.SetActive(value);
    }

    public void ToBack()
    {
      if (!enabled) return;
      if (!IsInBack())
        MovingObject.SetAsFirstSibling();
      ToggleGroups(false);
      SentToBack?.Invoke();
    }
    bool lastInFront;
    protected virtual void Update()
    {
      if (!enabled) return;
      bool inFront = IsInFront();
      ToggleGroups(inFront);
      if (inFront && !lastInFront)
        SentToFront?.Invoke();
      if (!inFront && lastInFront)
        SentToBack?.Invoke();
      lastInFront = inFront;
    }

    public bool IsInBack() => MovingObject.GetSiblingIndex() == 0;
    public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;
  }
}