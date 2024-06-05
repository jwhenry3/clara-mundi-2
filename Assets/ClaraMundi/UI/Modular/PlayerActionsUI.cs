using UnityEngine;

namespace ClaraMundi
{
  public class PlayerActionsUI : MonoBehaviour
  {
    public static PlayerActionsUI Instance;
    private Player player;

    public Transform ActionsContainer;
    public ActionUI ActionPrefab;
    public WindowUI ActionMenu;


    void Start()
    {
      Instance = this;
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
      LoadActions();
    }

    void LoadActions()
    {
      foreach (Transform child in ActionsContainer)
        Destroy(child.gameObject);
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
        if (actions.IndexOf(action) == 0)
          instance.button.Select();
      }
    }
  }
}