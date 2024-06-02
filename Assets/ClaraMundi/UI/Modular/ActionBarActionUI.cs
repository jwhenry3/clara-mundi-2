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


    protected override void LateUpdate()
    {
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