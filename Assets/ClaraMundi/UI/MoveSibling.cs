using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
      if (MoveObjectToBackOnClick)
        ToBack();
      else
        ToFront();
    }



    public void ToFront()
    {
      if (!IsInFront())
        MovingObject.SetAsLastSibling();
      if (!gameObject.activeInHierarchy)
        gameObject.SetActive(true);
      if (CanvasGroupToToggle != null)
      {
        CanvasGroupToToggle.interactable = true;
        CanvasGroupToToggle.blocksRaycasts = true;
      }
      SentToFront?.Invoke();
    }

    public void ToBack()
    {
      if (!IsInBack())
        MovingObject.SetAsFirstSibling();
      if (CanvasGroupToToggle != null)
      {
        CanvasGroupToToggle.interactable = false;
        CanvasGroupToToggle.blocksRaycasts = false;
      }
      SentToBack?.Invoke();
    }
    bool lastInFront;
    protected virtual void Update()
    {
      bool inFront = IsInFront();
      if (CanvasGroupToToggle != null)
      {
        bool sentToFront = false;
        bool sentToBack = false;
        if (inFront && !lastInFront)
        {
          sentToFront = true;
        }
        if (!inFront && lastInFront)
        {
          sentToBack = true;
        }
        CanvasGroupToToggle.interactable = inFront;
        CanvasGroupToToggle.blocksRaycasts = inFront;
        if (sentToFront)
          SentToFront?.Invoke();
        if (sentToBack)
          SentToBack?.Invoke();
      }
      foreach (var group in CanvasGroupsToToggle)
      {
        group.interactable = inFront;
        group.blocksRaycasts = inFront;
      }
      foreach (var obj in GameObjectsToToggle)
        obj.SetActive(inFront);
      lastInFront = inFront;

    }

    public bool IsInBack() => MovingObject.GetSiblingIndex() == 0;
    public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;
  }
}