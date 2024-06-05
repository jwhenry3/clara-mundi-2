using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
  public class ActionBarUI : MonoBehaviour
  {
    public ActionUI DraggingAction;
    public static ActionBarUI Instance;
    public MoveSibling ActionBarsSibling;
    public CanvasGroupWatcher ActionBar1;
    public CanvasGroupWatcher ActionBar2;

    public ButtonUI FirstAction1;
    public ButtonUI FirstAction2;

    public WindowUI ActionBarActionMenu;
    public WindowUI ActionMenu;
    public ActionUI CurrentAction;
    public ActionBarAction CurrentActionBarAction;

    public Transform Placeholder;

    void OnEnable()
    {
      Instance = this;
      if (InputManager.Instance == null) return;
      InputManager.Instance.Actions.FindAction("ActionBar1").performed += OnActionBar1;
      InputManager.Instance.Actions.FindAction("ActionBar2").performed += OnActionBar2;
    }

    public void OnActionMenuCancel()
    {
      CurrentAction = null;
    }

    private void OnActionBar2(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
      Placeholder.SetAsLastSibling();
      ActionBarsSibling.ToFront();
      StartCoroutine(SelectDelayed(FirstAction2));
    }

    private void OnActionBar1(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
      Placeholder.SetAsLastSibling();
      ActionBarsSibling.ToFront();
      StartCoroutine(SelectDelayed(FirstAction1));
    }

    IEnumerator SelectDelayed(ButtonUI button)
    {
      yield return new WaitForSeconds(0.1f);
      button.Select();
    }

    void OnDisable()
    {
      if (InputManager.Instance == null) return;
      InputManager.Instance.Actions.FindAction("ActionBar1").performed -= OnActionBar1;
      InputManager.Instance.Actions.FindAction("ActionBar2").performed -= OnActionBar2;
    }

    public void Assign()
    {
      if (CurrentAction == null) return;
      EventSystem.current.SetSelectedGameObject(null);
      ActionMenu.moveSibling.ToBack();
      ActionBarActionMenu.moveSibling.ToBack();
      ActionBarsSibling.ToFront();
      if (CurrentAction is ActionBarActionUI)
        CurrentAction.button.Select();
      else
        FirstAction1.Select();
    }
    public void Remove()
    {
      if (CurrentAction == null) return;
      CurrentAction.ChangeActionBarAction(new()
      {
        SlotName = CurrentAction.ActionBarAction.SlotName
      });

      EventSystem.current.SetSelectedGameObject(null);
      ActionMenu.moveSibling.ToBack();
      ActionBarActionMenu.moveSibling.ToBack();
      ActionBarsSibling.ToFront();

      CurrentAction.button.Select();

      CurrentAction = null;
    }

    public void Use()
    {
      if (CurrentAction == null) return;
      if (CurrentAction.Action != null && CurrentAction.ActionBarAction != null)
        ActionController.Instance.TriggerCommand(CurrentAction.Action.Command, CurrentAction.ActionBarAction.ActionArgs);
      else if (CurrentAction.Macro != null && CurrentAction.ActionBarAction != null)
      {
        // split macro by newline and execute each line in a particular order with a slight delay
      }
      CurrentAction = null;
      EventSystem.current.SetSelectedGameObject(null);
      ActionMenu.moveSibling.ToBack();
      ActionBarActionMenu.moveSibling.ToBack();
      ActionBarsSibling.ToBack();
    }

    void Update()
    {
      if (!ActionMenu.gameObject.activeInHierarchy && !ActionBarActionMenu.gameObject.activeInHierarchy && ActionBarsSibling.IsInBack() && CurrentAction != null)
      {
        CurrentAction = null;
      }
    }
  }
}