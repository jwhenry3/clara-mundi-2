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
    public Dictionary<string, ActionArg> Args;
    public string Text;
  }

  [Serializable]
  public class ActionBarAction
  {
    public string SlotName;
    public EntityAction Action;
    public MacroAction Macro;
    public string ItemId;

    public Dictionary<string, string> ActionArgs;

    public ActionBarAction Clone(string slotName)
    {
      return new()
      {
        SlotName = slotName,
        Action = Action,
        Macro = Macro,
        ItemId = ItemId,
        ActionArgs = new(ActionArgs ?? new())
      };
    }
  }
  [Serializable]
  public class MacroAction
  {
    public Sprite Icon;
    public string Name;
    public string Instructions;
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

    public ActionRepo ActionRepo => RepoManager.Instance.ActionRepo;

    public event Action<ActionInvocation> OnAction;

    public override void OnStartClient()
    {
      base.OnStartClient();
      if (PlayerManager.Instance.LocalPlayer == player)
        Instance = this;
    }

    public bool InvokeAction(string command, string text)
    {
      if (!IsServerStarted) return false;
      EntityAction action = ActionRepo.Get(command);

      if (action != null)
      {
        Dictionary<string, ActionArg> args = new();
        if (action.Args.Count > 0)
        {
          var words = text.Split(" ").ToList();
          foreach (var arg in action.Args)
          {
            if (words.Count > 0)
            {
              args[arg.Name] = new()
              {
                Name = arg.Name,
                Type = arg.Type,
                Value = words[0]
              };
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
        return true;
      }
      return false;
    }

    [ServerRpc()]
    public void TriggerCommand(string command, string text)
    {
      if (!InvokeAction(command, text))
      {
        player.Chat.Channel.ServerSendMessage(new()
        {
          Type = ChatMessageType.Error,
          Channel = "System",
          Message = "Invalid command."
        });
      }
    }
    [ServerRpc()]
    public void TriggerCommand(string command, Dictionary<string, string> actionArgs)
    {

      EntityAction action = ActionRepo.Get(command);
      if (action == null)
      {
        player.Chat.Channel.ServerSendMessage(new()
        {
          Type = ChatMessageType.Error,
          Channel = "System",
          Message = "Invalid command."
        });
        return;
      }

      Dictionary<string, ActionArg> args = new();
      if (action.Args.Count > 0)
      {
        foreach (var arg in action.Args)
        {
          if (actionArgs.ContainsKey(arg.Name))
          {
            args.Add(arg.Name, new()
            {
              Name = arg.Name,
              Type = arg.Type,
              Value = actionArgs[arg.Name]
            });
          }
        }
      }
      ActionInvocation invocation = new()
      {
        player = player,
        Action = action,
        Args = args,
        Text = "",
      };
      OnAction?.Invoke(invocation);
      ChatManager.Instance?.OnAction(invocation);
    }
  }
}