using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class ClickEvent : MonoBehaviour, IPointerDownHandler
  {

    public UnityEvent OnClick;

    public void OnPointerDown(PointerEventData eventData)
    {
      OnClick?.Invoke();
    }
  }
}