using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class MoveToFront : HoverDisablesInputs, IPointerDownHandler
  {
    public Transform MovingObject;

    public void Awake()
    {
    }

    public void OnPointerDown(PointerEventData eventData) => Move();
    public void Move()
    {
    }

    public void SelectFirstInteractable()
    {
    }

    public bool IsInFront() => MovingObject.GetSiblingIndex() == MovingObject.parent.childCount - 1;


    private float updateTick;
    private void Update()
    {

    }
  }
}