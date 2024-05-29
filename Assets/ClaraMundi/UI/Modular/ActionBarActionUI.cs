using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ActionBarActionUI : MonoBehaviour
  {
    public ButtonUI button;
    public TextMeshProUGUI InputText;
    public bool IsActionBar1;
    public bool IsActionBar2;

    private Player player;

    private InputAction actionBarInputAction;
    private InputAction action;

    private float interval = 0.2f;
    private float tick = 0;

    void OnEnable()
    {
      button = button ?? GetComponent<ButtonUI>();
      if (PlayerManager.Instance == null) return;
      player = PlayerManager.Instance.LocalPlayer;
    }

    void LateUpdate()
    {
      tick += Time.deltaTime;
      if (tick > interval)
      {
        tick = 0;
        if (InputManager.Instance != null)
        {
          actionBarInputAction = actionBarInputAction ?? InputManager.Instance.Actions.FindAction(IsActionBar1 ? "ActionBar1" : "ActionBar2");
          action = action ?? InputManager.Instance.Actions.FindAction(gameObject.name);
          string actionBarText = actionBarInputAction.GetBindingDisplayString();
          if (actionBarText == "Control")
            actionBarText = "Ctrl";
          InputText.text = actionBarText + " " + action.GetBindingDisplayString();
        }
        button.UseNameAsText = false;
        var slot = player.Actions.ActionBar1.Get(gameObject.name);
        if (IsActionBar2)
          slot = player.Actions.ActionBar2.Get(gameObject.name);
        if (slot != null)
        {
          var action = slot.Value;
          if (action.action != null)
          {
            button.iconSprite = slot.Value.action.Sprite;
            if (button.iconSprite == null)
            {
              button.text.text = slot.Value.action.Name;
              button.HasIcon = false;
              button.HasText = true;
            }
            else
            {
              button.HasIcon = true;
              button.HasText = false;
            }
            return;
          }
          else
          {
            button.HasIcon = false;
            button.HasText = true;
            button.text.text = !string.IsNullOrEmpty(action.MacroName) ? action.MacroName : "Macro";
            // create a name for the macro and display it
          }
        }
        button.HasIcon = false;
        button.HasText = false;
      }
    }
  }
}