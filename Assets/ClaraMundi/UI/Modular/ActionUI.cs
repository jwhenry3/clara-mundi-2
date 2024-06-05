using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace ClaraMundi
{
  public class ActionUI : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
  {
    public ActionUI DraggingAction => ActionBarUI.Instance.DraggingAction;
    public bool isDraggable;
    public bool CanSpawnDraggable;
    public WindowUI ActionMenu;

    public GameObject Highlight;
    public ActionBarAction ActionBarAction;
    public EntityAction Action => ActionBarAction?.Action ?? null;
    public MacroAction Macro => ActionBarAction?.Macro ?? null;
    public string ItemId => ActionBarAction?.ItemId ?? null;

    public ButtonUI button;

    protected PointerEventData pointerEventData;


    public bool IsActionBar1;
    public bool IsActionBar2;

    protected Player player;

    public ActionUI Origin;

    public float pointerTime;

    public InventoryItemUI InventoryItemUI;

    void OnEnable()
    {
      ActionBarAction ??= new();
      InventoryItemUI = InventoryItemUI ?? GetComponent<InventoryItemUI>();
      button = button ?? GetComponent<ButtonUI>();
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
      InputManager.Instance.UI.FindAction("Submit").performed += OnClick;
    }

    void OnDisable()
    {
      InputManager.Instance.UI.FindAction("Submit").performed -= OnClick;
    }
    protected virtual void OnClick(InputAction.CallbackContext context)
    {
      if (EventSystem.current.currentSelectedGameObject != gameObject) return;
      if (!isDraggable)
      {
        if (ActionMenu != null)
        {
          ActionMenu.moveSibling.ToFront();
          ActionMenu.parent = GetComponentInParent<WindowUI>();
          ActionBarUI.Instance.ActionBarsSibling.ToBack();
          ActionBarUI.Instance.CurrentAction = this;
        }
      }
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
        return player.Actions.ActionBar2;
      if (IsActionBar1)
        return player.Actions.ActionBar1;
      return null;
    }

    void PrepareActionBarAction()
    {
      var actionBar = GetActionBar();
      if (actionBar == null) return;
      ActionBarAction = GetActionBar().Get(gameObject.name);
    }
    public void ChangeActionBarAction(ActionBarAction action)
    {
      GetActionBar()?.Set(gameObject.name, action);
    }
    void OnDrop()
    {
      pointerTime = 0;
      SetActionBarAction();
      ActionBarAction = null;
      ActionBarUI.Instance.CurrentAction = null;
      gameObject.SetActive(false);
      ActionBarUI.Instance.ActionBarsSibling.ToBack();
    }
    void PrepareAction()
    {
      if (!string.IsNullOrEmpty(ActionBarAction.ItemId))
      {
        var item = RepoManager.Instance.ItemRepo.GetItem(ActionBarAction.ItemId);
        button.iconSprite = item.Icon;
        button.icon.sprite = item.Icon;
        button.HasIcon = true;
        button.HasText = false;
      }
      else if (Action.Sprite != null)
      {
        button.iconSprite = Action.Sprite;
        button.icon.sprite = Action.Sprite;
        button.HasIcon = true;
        button.HasText = false;
      }
      else if (Action != null && button.iconSprite == null)
      {
        button.text.text = Action.Name;
        button.HasIcon = false;
        button.HasText = true;
      }
      else
      {
        button.HasIcon = false;
        button.HasText = false;
        button.text.text = "";
      }
    }
    void PrepareMacro()
    {
      button.HasIcon = false;
      button.text.text = !string.IsNullOrEmpty(Macro.Name) ? Macro.Name : "Macro";
      button.HasText = true;
    }
    void PrepareUndefined()
    {
      button.HasIcon = false;
      button.HasText = false;
      button.text.text = "";
    }
    void PrepareButton()
    {
      button.UseNameAsText = false;
      if (Action != null)
        PrepareAction();
      else if (Macro != null && (!string.IsNullOrEmpty(Macro.Name) || !string.IsNullOrEmpty(Macro.Instructions)))
        PrepareMacro();
      else
        PrepareUndefined();
    }

    protected virtual void LateUpdate()
    {
      if (Highlight != null)
        Highlight.SetActive(ActionBarUI.Instance.CurrentAction == this);

      if (isDraggable && !Input.GetMouseButton(0))
        OnDrop();

      PrepareActionBarAction();
      if (InventoryItemUI == null && button != null)
        PrepareButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if (!isDraggable) return;
      transform.position = eventData.position;
      pointerEventData = eventData;
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
            if (pointerTime < 0.3f)
            {
              pointerTime += Time.deltaTime;
              return;
            }
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
          DraggingAction.ActionBarAction = ActionBarAction.Clone(ActionBarAction.SlotName);
          DraggingAction.gameObject.SetActive(true);
          DraggingAction.transform.position = transform.position;
          ActionBarUI.Instance.ActionBarsSibling.ToFront();
          ActionBarUI.Instance.CurrentAction = this;
        }
      }
    }
  }
}