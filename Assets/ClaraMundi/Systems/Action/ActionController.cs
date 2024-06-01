using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace ClaraMundi
{
  public struct ActionInvocation
  {
    public Player player;
    public EntityAction Action;
    public Dictionary<string, string> Args;
    public string Text;
  }

  [Serializable]
  public class ActionBarAction
  {
    public string SlotName;
    public EntityAction action;
    public string MacroName;
    public string Macro;
  }

  [Serializable]
  public class ActionBar : IndexedList<string, ActionBarAction>
  {
    public int Index = 0;
    [SerializeField]
    public List<ActionBarAction> Actions => Items;
    public ActionBar() : base("SlotName")
    {
    }

    public override bool FindPredicate(ActionBarAction item, string by)
    {
      return item.SlotName == by;
    }

    public override ActionBarAction Create(string by)
    {
      var instance = base.Create(by);
      instance.SlotName = by;
      return instance;
    }
  }
  [Serializable]
  public class CharacterClassActions : IndexedList<int, ActionBar>
  {
    public string ClassId;

    [SerializeField]
    public List<ActionBar> ActionBars => Items;

    public CharacterClassActions() : base("Index")
    {
    }

    public override bool FindPredicate(ActionBar item, int by)
    {
      return item.Index == by;
    }
    public override ActionBar Create(int by)
    {
      var instance = base.Create(by);
      instance.Index = by;
      return instance;
    }
  }

  [Serializable]
  public class ActionBarCollection : IndexedList<string, CharacterClassActions>
  {
    [SerializeField]
    public List<CharacterClassActions> ClassActionBars => Items;
    public ActionBarCollection() : base("ClassId")
    {

    }
    public override bool FindPredicate(CharacterClassActions item, string by)
    {
      return item.ClassId == by;
    }
    public override CharacterClassActions Create(string by)
    {
      var instance = base.Create(by);
      instance.ClassId = by;
      return instance;
    }
  }
  public class ActionController : PlayerController
  {
    public static ActionController Instance;

    [SerializeField]
    public ActionBarCollection ActionBarCollection;
    public int ActionBar1Index = 0;
    public int ActionBar2Index = 1;

    public ActionBar ActionBar1 => ActionBarCollection.Get(player.Character.classId ?? "").Get(ActionBar1Index);
    public ActionBar ActionBar2 => ActionBarCollection.Get(player.Character.classId ?? "").Get(ActionBar2Index);

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
    public void TriggerCommand(string command, string text)
    {
      EntityAction action = null;
      if (ActionsByCommand.ContainsKey(command))
        action = ActionsByCommand[command];
      if (ActionsByShortCommand.ContainsKey(command))
        action = ActionsByShortCommand[command];
      if (action != null)
      {
        Dictionary<string, string> args = new();
        if (action.ArgNames.Length > 0)
        {
          var words = text.Split(" ").ToList();
          foreach (var arg in action.ArgNames)
          {
            if (words.Count > 0)
            {
              args[arg] = words[0];
              words.RemoveAt(0);
            }
          }
          text = string.Join(" ", words);
        }
        ActionInvocation invocation = new()
        {
          player = player,
          Action = action,
          Args = args,
          Text = text,
        };
        OnAction?.Invoke(invocation);
        ChatManager.Instance?.OnAction(invocation);
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