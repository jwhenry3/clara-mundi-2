using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace ClaraMundi
{
  [Serializable]
  public class ActionBarActions : Dictionary<string, ActionBarAction>
  {
  }

  [Serializable]
  public struct ActionBarAction
  {
    public EntityAction action;
    public string Macro;
  }
  public struct ActionInvocation
  {
    public EntityAction Action;
    public string CommandArg1;
    public string CommandArg2;
  }
  [Serializable]
  public struct ActionBarSlot
  {
    public string Key;
    public ActionBarAction Value;
  }
  [Serializable]
  public class ActionBar
  {
    public List<ActionBarSlot> ActionsList;
  }
  public class ActionController : PlayerController
  {
    public static ActionController Instance;

    [SerializeField]
    public List<ActionBar> ActionBars;
    public int ActionBar1Index = 0;
    public int ActionBar2Index = 1;

    public ActionBar ActionBar1 => ActionBars[ActionBar1Index];
    public ActionBar ActionBar2 => ActionBars[ActionBar2Index];

    public List<EntityAction> Actions;
    public Dictionary<string, EntityAction> ActionsByCommand;
    public Dictionary<string, EntityAction> ActionsByShortCommand;
    public Dictionary<string, EntityAction> ActionsById;

    public event Action<ActionInvocation> OnAction;

    public override void OnStartClient()
    {
      base.OnStartClient();
      if (PlayerManager.Instance.LocalPlayer == player)
        Instance = this;
    }
    void OnEnable()
    {
      Actions = Actions ?? new();
      ActionsById = new();
      ActionsByCommand = new();
      ActionsByShortCommand = new();
      foreach (var action in Actions)
      {
        ActionsByCommand[action.Command] = action;
        ActionsByShortCommand[action.CommandShort] = action;
        ActionsById[action.Id] = action;
      }
    }

    [ServerRpc()]
    public void TriggerCommand(string command, string CommandArg1 = "", string CommandArg2 = "")
    {
      if (ActionsByCommand.ContainsKey(command))
      {
        OnAction?.Invoke(new()
        {
          Action = ActionsByCommand[command],
          CommandArg1 = CommandArg1,
          CommandArg2 = CommandArg2
        });
      }
      else if (ActionsByShortCommand.ContainsKey(command))
      {
        OnAction?.Invoke(new()
        {
          Action = ActionsByShortCommand[command],
          CommandArg1 = CommandArg1,
          CommandArg2 = CommandArg2
        });
      }
      else
      {
        player.Chat.Channel.ServerSendMessage(new()
        {
          Type = ChatMessageType.System,
          Channel = "System",
          Message = "Invalid command"
        });
      }
    }
  }
}