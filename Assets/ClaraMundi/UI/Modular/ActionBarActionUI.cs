using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ActionBarActionUI : ActionUI, IPointerMoveHandler
  {
    public TextMeshProUGUI InputText;

    private InputAction actionBarInputAction;
    private InputAction action;


    void OnEnable()
    {
      button = button ?? GetComponent<ButtonUI>();
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
      InputManager.Instance.UI.FindAction("Submit").performed += OnClick;
    }

    void OnDisable()
    {
      InputManager.Instance.UI.FindAction("Submit").performed -= OnClick;
    }
    protected override void LateUpdate()
    {
      if (ActionBarUI.Instance.ActionBarsSibling.IsInBack())
      {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
          EventSystem.current.SetSelectedGameObject(null);
        if (ActionMenu != null && !ActionMenu.gameObject.activeInHierarchy && ActionBarUI.Instance.CurrentAction == this)
          ActionBarUI.Instance.CurrentAction = null;
      }
      CanSpawnDraggable = true;
      if (InputManager.Instance != null)
      {
        actionBarInputAction = actionBarInputAction ?? InputManager.Instance.Actions.FindAction(IsActionBar1 ? "ActionBar1" : "ActionBar2");
        action = action ?? InputManager.Instance.Actions.FindAction(gameObject.name);
        string actionBarText = actionBarInputAction.GetBindingDisplayString();
        if (actionBarText == "Control")
          actionBarText = "Ctrl";
        InputText.text = actionBarText + " " + action.GetBindingDisplayString();
      }
      base.LateUpdate();
    }

    protected override void OnClick(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject != gameObject) return;
      if (!isDraggable)
      {
        if (ActionBarUI.Instance.ActionBarActionMenu != null)
        {
          if (ActionBarUI.Instance.CurrentAction != null)
          {
            OnSet(ActionBarUI.Instance.CurrentAction);
            ActionBarUI.Instance.CurrentAction = null;
            if (!UIHandling.Instance.IsPlaceholderLastSibling())
              ActionBarUI.Instance.ActionBarsSibling.ToBack();
            return;
          }
          ActionBarUI.Instance.ActionBarsSibling.ToBack();
          ActionBarUI.Instance.ActionBarActionMenu.transform.position = transform.position;
          ActionBarUI.Instance.ActionBarActionMenu.moveSibling.ToFront();
          ActionBarUI.Instance.CurrentAction = this;
        }
      }
    }
    public void OnSet(ActionUI ActionUI)
    {
      if (ActionUI != this)
      {
        var macro = ActionUI.Macro;
        var action = ActionUI.Action;
        // change the player's action to this slot
        ActionBar previousActionBar = ActionUI.GetActionBar();
        ActionBar nextActionBar = GetActionBar();
        if (previousActionBar != null)
        {
          var previousSlot = ActionUI.ActionBarAction != null ? previousActionBar.Get(ActionUI.ActionBarAction.SlotName) : null;
          if (previousSlot != null)
          {
            var previousSlotName = previousSlot.SlotName;
            previousActionBar.Set(previousSlotName, new()
            {
              SlotName = previousSlotName,
              Action = Action,
              Macro = Macro
            });

            previousSlot.SlotName = ActionBarAction.SlotName;
          }
        }
        nextActionBar.Set(ActionBarAction.SlotName, new()
        {
          SlotName = ActionBarAction.SlotName,
          Action = action,
          Macro = macro
        });
      }
      button.Select();
    }
  }
}