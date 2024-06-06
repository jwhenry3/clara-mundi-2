using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class PlayerActionsUI : MonoBehaviour
  {
    public static PlayerActionsUI Instance;
    private Player player;

    private WindowUI window;

    public Transform ActionsContainer;
    public ActionUI ActionPrefab;
    public WindowUI ActionMenu;

    public GameObject GeneralButtonHighlight;
    public GameObject ClassButtonHighlight;
    public GameObject SocialButtonHighlight;

    public List<EntityAction> GeneralActions;
    public List<EntityAction> SocialActions;
    public List<ClassAction> ClassActions => player.Stats.ClassType.Actions;
    public string ActionType = "Class";

    public CanvasGroup CategoryCanvasGroup;
    public CanvasGroup ListCanvasGroup;
    public CanvasGroupWatcher ListWatcher;

    public void SetActionType(string actionType)
    {
      ActionType = actionType;
      // ListCanvasGroup.interactable = true;
      // CategoryCanvasGroup.interactable = false;
      LoadActions();
    }
    void Start()
    {
      window = GetComponentInParent<WindowUI>();
      Instance = this;
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
    }

    void OnEnable()
    {
      // ListCanvasGroup.interactable = false;
      // CategoryCanvasGroup.interactable = true;
      LoadActions();
    }

    void LoadActions(bool selectFirst = false)
    {
      foreach (Transform child in ActionsContainer)
        Destroy(child.gameObject);

      if (ActionType == "Class")
        LoadClassActions(selectFirst);
      else if (ActionType == "General")
        LoadEntityActions(GeneralActions, selectFirst);
      else if (ActionType == "Social")
        LoadEntityActions(SocialActions, selectFirst);
    }

    void LateUpdate()
    {
      GeneralButtonHighlight.SetActive(ActionType == "General");
      ClassButtonHighlight.SetActive(ActionType == "Class");
      SocialButtonHighlight.SetActive(ActionType == "Social");
    }
    void LoadEntityActions(List<EntityAction> actions, bool selectFirst = false)
    {
      foreach (var action in actions)
      {
        var instance = Instantiate(ActionPrefab, ActionsContainer);
        instance.ActionBarAction = new()
        {
          Action = action,
        };
        instance.ActionMenu = ActionMenu;
        if (actions.IndexOf(action) == 0 && selectFirst)
          instance.button.Select();
      }
    }
    void LoadClassActions(bool selectFirst = false)
    {
      if (player == null || player.Stats.ClassType == null) return;
      var actions = player.Stats.ClassType.Actions;
      foreach (var action in actions)
      {
        var instance = Instantiate(ActionPrefab, ActionsContainer);
        instance.ActionBarAction = new()
        {
          Action = action.Action,
        };
        instance.ActionMenu = ActionMenu;
        if (actions.IndexOf(action) == 0 && selectFirst)
          instance.button.Select();
      }
    }
  }
}