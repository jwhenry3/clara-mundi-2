using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class MoveSibling : MonoBehaviour, IPointerDownHandler
  {
    public Transform MovingObject;
    public CanvasGroup CanvasGroupToToggle;

    public Action SentToBack;
    public Action SentToFront;

    public bool MoveObjectToBackOnClick;
    public void OnPointerDown(PointerEventData eventData)
    {
      if (MoveObjectToBackOnClick)
        ToBack();
      else
        ToFront();
    }

    public void ToFront()
    {
      if (!IsInFront())
        MovingObject.SetAsLastSibling();
      SentToFront?.Invoke();
    }

    public void ToBack()
    {
      if (!IsInBack())
        MovingObject.SetAsFirstSibling();
      SentToBack?.Invoke();
    }

    void Update()
    {
      if (CanvasGroupToToggle != null)
      {
        CanvasGroupToToggle.interactable = IsInFront();
      }
    }

    public bool IsInBack() => MovingObject.GetSiblingIndex() == 0;
    public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;
  }
}