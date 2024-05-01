﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class MoveSibling : MonoBehaviour, IPointerDownHandler
  {
    public Transform MovingObject;
    public CanvasGroup CanvasGroupToToggle;
    public CanvasGroup[] CanvasGroupsToToggle;

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
      if (CanvasGroupToToggle != null)
        CanvasGroupToToggle.interactable = true;
      SentToFront?.Invoke();
    }

    public void ToBack()
    {
      if (!IsInBack())
        MovingObject.SetAsFirstSibling();
      if (CanvasGroupToToggle != null)
        CanvasGroupToToggle.interactable = false;
      SentToBack?.Invoke();
    }

    void Update()
    {
      bool inFront = IsInFront();
      if (CanvasGroupToToggle != null)
        CanvasGroupToToggle.interactable = inFront;
      foreach (var group in CanvasGroupsToToggle)
        group.interactable = inFront;

    }

    public bool IsInBack() => MovingObject.GetSiblingIndex() == 0;
    public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;
  }
}