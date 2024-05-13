using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class TabNavigation : MonoBehaviour
  {

    public CanvasGroupFocus focus;
    public Selectable selectable;


    private bool listening;
    private bool hasPressedNext;
    private bool hasPressedPrevious;
    private float inputTimer;

    public void Listen()
    {
      if (!listening)
      {
        InputManager.Instance.UI.FindAction("NextElement").performed += OnNext;
        InputManager.Instance.UI.FindAction("PreviousElement").performed += OnPrevious;
        listening = true;
      }
    }
    public void Listen(Action<InputAction.CallbackContext> OnSubmit)
    {
      if (!listening)
      {
        Listen();
        InputManager.Instance.UI.FindAction("Submit").performed += OnSubmit;
        listening = true;
      }
    }
    public void StopListening()
    {

      if (focus != null && listening)
      {
        InputManager.Instance.UI.FindAction("NextElement").performed -= OnNext;
        InputManager.Instance.UI.FindAction("PreviousElement").performed -= OnPrevious;
        listening = false;
      }
    }

    public void StopListening(Action<InputAction.CallbackContext> OnSubmit)
    {

      if (focus != null && listening)
      {
        StopListening();
        InputManager.Instance.UI.FindAction("Submit").performed -= OnSubmit;
        listening = false;
      }
    }

    void OnNext(InputAction.CallbackContext context)
    {
      hasPressedNext = true;
      inputTimer = 0.1f;
    }

    void OnPrevious(InputAction.CallbackContext context)
    {
      hasPressedPrevious = true;
      inputTimer = 0.1f;
    }

    public void Update()
    {
      if (inputTimer > 0)
      {
        inputTimer -= Time.deltaTime;
        if (inputTimer <= 0)
        {
          if (hasPressedPrevious)
          {
            hasPressedPrevious = false;
            hasPressedNext = false;
            var previous = selectable.FindSelectableOnLeft() ?? selectable.FindSelectableOnUp();
            if (previous != null)
              EventSystem.current.SetSelectedGameObject(previous.gameObject);
          }
          else if (hasPressedNext)
          {
            hasPressedPrevious = false;
            hasPressedNext = false;
            var next = selectable.FindSelectableOnRight() ?? selectable.FindSelectableOnDown();
            if (next != null)
              EventSystem.current.SetSelectedGameObject(next.gameObject);
          }
        }
      }
    }

  }
}