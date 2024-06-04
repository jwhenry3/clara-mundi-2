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
          if (ActionBarUI.Instance.CurrentActionBarAction != null)
          {
            OnSet(ActionBarUI.Instance.CurrentActionBarAction);
            ActionBarUI.Instance.CurrentActionBarAction = null;
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
    public void OnSet(ActionBarAction actionBarAction)
    {
      if (actionBarAction != this.ActionBarAction)
      {
        var macro = actionBarAction.Macro;
        var action = actionBarAction.Action;
        var item = RepoManager.Instance.ItemRepo.GetItem(actionBarAction.ItemId);

        ActionBar nextActionBar = GetActionBar();
        nextActionBar.Set(ActionBarAction.SlotName, new()
        {
          SlotName = ActionBarAction.SlotName,
          Action = action,
          Macro = macro,
          ItemId = item != null ? item.ItemId : null,
          ActionArgs = actionBarAction.ActionArgs,
        });
      }
      button.Select();
    }
    public void OnSet(ActionUI ActionUI)
    {
      if (ActionUI != this)
      {
        var macro = ActionUI.Macro;
        var action = ActionUI.Action;
        var item = ActionUI.Item;
        var args = ActionUI.ActionBarAction != null ? ActionUI.ActionBarAction.ActionArgs : new();
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
              Macro = Macro,
              ItemId = Item != null ? Item.ItemId : null,
              ActionArgs = ActionBarAction.ActionArgs,
            });

            previousSlot.SlotName = ActionBarAction.SlotName;
          }
        }
        nextActionBar.Set(ActionBarAction.SlotName, new()
        {
          SlotName = ActionBarAction.SlotName,
          Action = action,
          Macro = macro,
          ItemId = item != null ? item.ItemId : null,
          ActionArgs = args
        });
      }
      button.Select();
    }
  }
}