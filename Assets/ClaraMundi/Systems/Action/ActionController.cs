using System;
using System.Collections.Generic;
using System.Linq;
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
    public string MacroName;
    public string Macro;
  }
  public struct ActionInvocation
  {
    public Player player;
    public EntityAction Action;
    public Dictionary<string, string> Args;
    public string Text;
  }
  [Serializable]
  public class ActionBarSlot
  {
    public string Key;
    public ActionBarAction Value;
  }
  [Serializable]
  public class ActionBar
  {
    public List<ActionBarSlot> ActionsList;
    public Dictionary<string, ActionBarSlot> ActionsDict;

    public ActionBarSlot Get(string name)
    {
      ActionsList = ActionsList ?? new();
      ActionsDict = ActionsDict ?? new();
      if (string.IsNullOrEmpty(name)) return new();
      ActionBarSlot existing = ActionsDict.ContainsKey(name) ? ActionsDict[name] : null;
      if (existing == null)
      {
        existing = ActionsList.Find((a) => a.Key == name);

        if (existing == null)
        {
          existing = new() { Key = name, Value = new() };
          ActionsList.Add(existing);
        }
        ActionsDict[name] = existing;
      }
      return ActionsDict[name];
    }
  }
  [Serializable]
  public class CharacterClassActionBars
  {
    public string classId;
    public List<ActionBar> ActionBars;

    public ActionBar Get(int index)
    {
      ActionBars = ActionBars ?? new();
      if (ActionBars.Count < index + 1)
      {
        for (int i = ActionBars.Count - 1; i < index + 1; i++)
          ActionBars.Add(new());
      }
      return ActionBars[index];
    }
  }

  [Serializable]
  public class ActionBarCollection
  {

    public List<CharacterClassActionBars> Collection;

    public Dictionary<string, CharacterClassActionBars> Dictionary = new();


    public CharacterClassActionBars Get(string classId)
    {
      if (string.IsNullOrEmpty(classId)) return new();
      var existing = Dictionary.ContainsKey(classId) ? Dictionary[classId] : null;
      if (existing == null)
      {
        existing = Collection.Find((a) => a.classId == classId);
        if (existing == null)
        {
          existing = new() { classId = classId, ActionBars = new() };
          Collection.Add(existing);
        }
        Dictionary[classId] = existing;
      }
      return existing;
    }
  }
  public class ActionController : PlayerController
  {
    public static ActionController Instance;

    [SerializeField]
    public ActionBarCollection ActionBarCollection;
    public int ActionBar1Index = 0;
    public int ActionBar2Index = 1;

    public ActionBar ActionBar1 => ActionBarCollection.Get(player.Character.classId).Get(ActionBar1Index);
    public ActionBar ActionBar2 => ActionBarCollection.Get(player.Character.classId).Get(ActionBar2Index);

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