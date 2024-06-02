using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ClaraMundi
{
  public class ActionUI : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
  {
    public ActionUI DraggingAction;
    public bool isDraggable;
    public bool CanSpawnDraggable;

    public ActionBarAction ActionBarAction;
    public EntityAction Action;
    public MacroAction Macro;

    public ButtonUI button;

    protected PointerEventData pointerEventData;

    public MoveSibling ActionBarMoveSibling;


    public bool IsActionBar1;
    public bool IsActionBar2;

    protected Player player;

    public ActionUI Origin;

    void OnEnable()
    {
      button = button ?? GetComponent<ButtonUI>();
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
    }
    void SetActionBarAction()
    {
      if (pointerEventData == null) return;
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(pointerEventData, results);

      foreach (var result in results)
      {
        var actionBarActionUI = result.gameObject.GetComponent<ActionBarActionUI>();
        if (actionBarActionUI != null)
          actionBarActionUI.OnSet(Origin ?? this);
      }
    }

    public ActionBar GetActionBar()
    {
      if (IsActionBar2)
      {
        return player.Actions.ActionBar2;
      }
      return player.Actions.ActionBar1;
    }

    protected virtual void LateUpdate()
    {
      button.UseNameAsText = false;
      if (IsActionBar1)
      {
        ActionBarAction = player.Actions.ActionBar1.Get(gameObject.name);
        Action = ActionBarAction.Action;
        Macro = ActionBarAction.Macro;
      }
      if (IsActionBar2)
      {
        ActionBarAction = player.Actions.ActionBar2.Get(gameObject.name);
        Action = ActionBarAction.Action;
        Macro = ActionBarAction.Macro;
      }
      if (isDraggable && !Input.GetMouseButton(0))
      {
        SetActionBarAction();
        ActionBarAction = null;
        Action = null;
        Macro = null;
        gameObject.SetActive(false);
        if (ActionBarMoveSibling != null)
          ActionBarMoveSibling.ToBack();
      }
      if (Action != null)
      {
        button.iconSprite = Action.Sprite;
        if (button.iconSprite == null)
        {
          button.text.text = Action.Name;
          button.HasIcon = false;
          button.HasText = true;
        }
        else
        {
          button.HasIcon = true;
          button.HasText = false;
          button.text.text = "";
        }
        return;
      }
      else if (Macro != null && (!string.IsNullOrEmpty(Macro.Name) || !string.IsNullOrEmpty(Macro.Instructions)))
      {
        button.HasIcon = false;
        button.text.text = !string.IsNullOrEmpty(Macro.Name) ? Macro.Name : "Macro";
        button.HasText = true;
        // create a name for the macro and display it
      }
      else
      {
        button.HasIcon = false;
        button.HasText = false;
        button.text.text = "";
      }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (isDraggable)
      {
        transform.position = eventData.position;
        pointerEventData = eventData;
      }
    }


    public void OnPointerMove(PointerEventData eventData)
    {
      if (isDraggable)
      {
        pointerEventData = eventData;
        transform.position = eventData.position;
      }
      else
      {
        if (CanSpawnDraggable && DraggingAction != null)
        {
          if (Input.GetMouseButton(0))
          {
            OnDrag(eventData);
          }
        }
      }
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (DraggingAction != null)
      {
        if (!DraggingAction.gameObject.activeInHierarchy)
        {
          DraggingAction.Origin = this;
          DraggingAction.ActionBarMoveSibling = ActionBarMoveSibling;
          if (ActionBarAction != null)
          {
            DraggingAction.ActionBarAction = ActionBarAction;
            DraggingAction.Action = ActionBarAction.Action;
            DraggingAction.Macro = ActionBarAction.Macro;
          }
          else
          {
            DraggingAction.Action = Action;
            DraggingAction.Macro = Macro;
          }
          DraggingAction.gameObject.SetActive(true);
          DraggingAction.transform.position = transform.position;
          ActionBarMoveSibling?.ToFront();
        }
      }
    }
  }
}