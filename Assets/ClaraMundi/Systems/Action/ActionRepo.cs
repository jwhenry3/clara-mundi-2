using UnityEngine;
using System;
using System.Collections.Generic;
namespace ClaraMundi
{
  [CreateAssetMenu(fileName = "ActionRepo", menuName = "Clara Mundi/Actions/ActionRepo")]
  [Serializable]
  public class ActionRepo : ScriptableObject
  {
    public List<EntityAction> ActionList;
    public Dictionary<string, EntityAction> ActionsById;
    public Dictionary<string, EntityAction> ActionsByCommand;
    public Dictionary<string, EntityAction> ActionsByShortCommand;

    public void OnEnable()
    {
      ActionsByCommand = new();
      ActionsByShortCommand = new();
      ActionsById = new();
      if (ActionList == null) return;
      foreach (var action in ActionList)
      {
        ActionsById.Add(action.Id, action);
        ActionsByCommand.Add(action.Command, action);
        ActionsByShortCommand.Add(action.CommandShort, action);
      }
    }

    public EntityAction Get(string value)
    {
      if (ActionsById.ContainsKey(value))
        return ActionsById[value];
      if (ActionsByCommand.ContainsKey(value))
        return ActionsByCommand[value];
      if (ActionsByShortCommand.ContainsKey(value))
        return ActionsByShortCommand[value];
      return null;
    }
  }
}