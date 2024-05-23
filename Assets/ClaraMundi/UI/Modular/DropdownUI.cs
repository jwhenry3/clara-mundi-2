using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class DropdownUI : MonoBehaviour, ISelectHandler
  {
    [HideInInspector]
    public WindowUI window;
    [HideInInspector]
    public TMP_Dropdown dropdown;
    private ScrollRect scroller;
    private GameObject lastSelected;
    private CanvasGroupWatcher canvasGroupWatcher;
    public bool AutoFocus
    {
      set
      {
        canvasGroupWatcher = canvasGroupWatcher ?? GetComponentInParent<CanvasGroupWatcher>(true);
        if (value == true)
        {
          if (canvasGroupWatcher.AutoFocusDropdown != null) return;
          canvasGroupWatcher.AutoFocusInput = null;
          canvasGroupWatcher.AutoFocusButton = null;
          canvasGroupWatcher.AutoFocusDropdown = this;
          if (canvasGroupWatcher.IsInteractable())
            Select();
        }
        else
        {
          if (canvasGroupWatcher.AutoFocusDropdown == this)
            canvasGroupWatcher.AutoFocusDropdown = null;
        }
      }
    }


    void Start()
    {
      canvasGroupWatcher = canvasGroupWatcher ?? GetComponentInParent<CanvasGroupWatcher>();
      scroller = scroller ?? GetComponentInParent<ScrollRect>();
      window = window ?? GetComponentInParent<WindowUI>();
      dropdown = dropdown ?? GetComponent<TMP_Dropdown>();

    }

    public void SnapTo(RectTransform child)
    {
      if (scroller == null) return;
      float padding = child.sizeDelta.y / 2;
      var scrollRect = scroller;
      float viewportHeight = scrollRect.viewport.rect.height;
      Vector2 scrollPosition = scrollRect.content.anchoredPosition;

      float elementTop = child.anchoredPosition.y;
      float elementBottom = elementTop - child.rect.height;


      float visibleContentTop = -scrollPosition.y - padding;
      float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

      float scrollDelta =
          elementTop > visibleContentTop ? visibleContentTop - elementTop :
          elementBottom < visibleContentBottom ? visibleContentBottom - elementBottom :
          0f;

      scrollPosition.y += scrollDelta;
      scrollRect.content.anchoredPosition = scrollPosition;
    }


    public void OnSelect(BaseEventData eventData)
    {
      if (canvasGroupWatcher != null)
      {
        canvasGroupWatcher.CurrentInput = null;
        canvasGroupWatcher.CurrentButton = null;
        canvasGroupWatcher.CurrentDropdown = this;
      }
    }

    void LateUpdate()
    {
      if (Application.isPlaying && EventSystem.current != null)
      {
        if (lastSelected != gameObject && EventSystem.current.currentSelectedGameObject == gameObject)
          SnapTo(transform as RectTransform);
        lastSelected = EventSystem.current.currentSelectedGameObject;
      }
    }
    public void Select()
    {
      EventSystem.current?.SetSelectedGameObject(gameObject);
    }
  }
}