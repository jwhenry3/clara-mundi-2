using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public enum FocusDirection
  {
    Vertical,
    Horizontal,
  }
  public class InputUI : MonoBehaviour, ISelectHandler
  {

    [HideInInspector]
    public FormUI formUI;
    [HideInInspector]
    public WindowUI window;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    [HideInInspector]
    public TMP_InputField inputField;

    public bool ClearOnDisable;

    public FocusDirection Direction;

    private bool nextPressed;
    private bool prevPressed;
    private float tick;
    private float interval = 0.1f;
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
          if (canvasGroupWatcher.AutoFocusInput != null) return;
          canvasGroupWatcher.AutoFocusInput = this;
          canvasGroupWatcher.AutoFocusButton = null;
          canvasGroupWatcher.AutoFocusDropdown = null;
          if (canvasGroupWatcher.IsInteractable())
            Select();
        }
        else
        {
          if (canvasGroupWatcher.AutoFocusInput == this)
            canvasGroupWatcher.AutoFocusInput = null;
        }
      }
    }


    void Start()
    {
      canvasGroupWatcher = canvasGroupWatcher ?? GetComponentInParent<CanvasGroupWatcher>();
      scroller = scroller ?? GetComponentInParent<ScrollRect>();
      formUI = formUI ?? GetComponentInParent<FormUI>();
      window = window ?? GetComponentInParent<WindowUI>();
      canvasGroup = canvasGroup ?? GetComponentInParent<CanvasGroup>();
      inputField = inputField ?? GetComponent<TMP_InputField>();
      inputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return DisableTabValidate(addedChar); };
    }
    void OnEnable()
    {

      if (InputManager.Instance != null)
      {
        InputManager.Instance.UI.FindAction("NextElement").performed += OnNextElement;
        InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
        InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPreviousElement;
      }
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
    void OnDisable()
    {
      if (ClearOnDisable)
        inputField.text = "";
      if (InputManager.Instance != null)
      {
        InputManager.Instance.UI.FindAction("NextElement").performed -= OnNextElement;
        InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
        InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPreviousElement;
      }
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject == gameObject)
        formUI.Submit();
    }

    private char DisableTabValidate(char charToValidate)
    {

      //Checks if a tab sign is entered....
      if (charToValidate == '\t')
      {
        // ... if it is change it to an empty character.
        charToValidate = '\0';
      }
      return charToValidate;
    }

    public void OnSelect(BaseEventData eventData)
    {
      if (canvasGroupWatcher != null)
      {
        canvasGroupWatcher.CurrentInput = this;
        canvasGroupWatcher.CurrentButton = null;
        canvasGroupWatcher.CurrentDropdown = null;
      }
    }

    void Update()
    {
      tick += Time.deltaTime;
      if (tick > interval)
      {
        tick = 0;
        if (nextPressed && !prevPressed)
          Next();
        else if (prevPressed)
          Prev();
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

    void Next()
    {
      nextPressed = false;
      prevPressed = false;
      if (Direction == FocusDirection.Horizontal)
      {
        var target = inputField.FindSelectableOnRight();
        if (target != null)
          EventSystem.current.SetSelectedGameObject(target.gameObject);
      }
      else
      {
        var target = inputField.FindSelectableOnDown();
        if (target != null)
          EventSystem.current.SetSelectedGameObject(target.gameObject);
      }
    }
    void Prev()
    {
      nextPressed = false;
      prevPressed = false;

      if (Direction == FocusDirection.Horizontal)
      {
        var target = inputField.FindSelectableOnLeft();
        if (target != null)
          EventSystem.current.SetSelectedGameObject(target.gameObject);
      }
      else
      {
        var target = inputField.FindSelectableOnUp();
        if (target != null)
          EventSystem.current.SetSelectedGameObject(target.gameObject);
      }
    }

    void OnNextElement(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject == gameObject)
        nextPressed = true;
    }

    void OnPreviousElement(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject == gameObject)
      {
        prevPressed = true;
        nextPressed = false;
      }
    }
    public void Select()
    {
      EventSystem.current?.SetSelectedGameObject(gameObject);
      inputField.ActivateInputField();
    }
  }
}