using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ActionBarActionUI : ActionUI, IPointerMoveHandler
  {
    public TextMeshProUGUI InputText;
    public TextMeshProUGUI QuantityText;


    private InputAction actionBarInputAction;
    private InputAction action;

    private float itemCheckInterval = 1;
    private float itemCheckTick = 0;


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
      if (!string.IsNullOrEmpty(ActionBarAction.ItemId))
      {
        itemCheckTick += Time.deltaTime;
        if (itemCheckTick > itemCheckInterval)
        {
          itemCheckTick = 0;
          int quantity = player.Inventory.ItemStorage.QuantityOf(ActionBarAction.ItemId);
          QuantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
      }
      else
      {
        QuantityText.text = "";
      }
      base.LateUpdate();
    }

    protected override void OnClick(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject != gameObject) return;
      if (isDraggable) return;
      if (ActionBarUI.Instance.ActionBarActionMenu == null) return;

      if (ActionBarUI.Instance.CurrentAction != null)
        OnSet(ActionBarUI.Instance.CurrentAction);
      else if (ActionBarUI.Instance.CurrentActionBarAction != null)
        OnSet(ActionBarUI.Instance.CurrentActionBarAction);
      else
        PrepareMove();
    }

    void PrepareMove()
    {
      ActionBarUI.Instance.ActionBarsSibling.ToBack();
      ActionBarUI.Instance.ActionBarActionMenu.transform.position = transform.position;
      ActionBarUI.Instance.ActionBarActionMenu.moveSibling.ToFront();
      ActionBarUI.Instance.CurrentAction = this;
    }

    public void OnSet(ActionBarAction actionBarAction)
    {
      ActionBarUI.Instance.CurrentActionBarAction = null;
      if (!UIHandling.Instance.IsPlaceholderLastSibling())
        ActionBarUI.Instance.ActionBarsSibling.ToBack();
      if (actionBarAction != this.ActionBarAction)
        GetActionBar()?.Set(ActionBarAction.SlotName, actionBarAction.Clone(ActionBarAction.SlotName));
      button.Select();
    }
    public void OnSet(ActionUI ActionUI)
    {
      ActionBarUI.Instance.CurrentAction = null;
      if (!UIHandling.Instance.IsPlaceholderLastSibling())
        ActionBarUI.Instance.ActionBarsSibling.ToBack();
      if (ActionUI != this)
      {
        ActionUI.GetActionBar()?.Set(ActionUI.ActionBarAction.SlotName, ActionBarAction.Clone(ActionUI.ActionBarAction.SlotName));
        GetActionBar()?.Set(ActionBarAction.SlotName, ActionUI.ActionBarAction.Clone(ActionBarAction.SlotName));
      }
      button.Select();
    }
  }
}