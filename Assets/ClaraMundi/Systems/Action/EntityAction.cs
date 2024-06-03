using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public enum ActionArgType
  {
    Entity,
    Player,
    NPC,
    Item,
    Skill,
    Spell,
    Generic,
  }
  [Serializable]
  public class ActionArg
  {
    public string Name;
    public ActionArgType Type;
    [HideInInspector]
    public string Value;

    public Item Item => Type == ActionArgType.Item ? RepoManager.Instance.ItemRepo.GetItem(Value) : null;
    public Entity Entity => Type == ActionArgType.Entity ? EntityManager.Instance.GetEntity(Value) : null;
    public NPC NPC => Type == ActionArgType.Entity || Type == ActionArgType.NPC ? NPCManager.Instance.GetNPC(Value) : null;
    public Player Player => Type == ActionArgType.Entity || Type == ActionArgType.Player ? PlayerManager.Instance.GetPlayer(Value) : null;
  }
  [CreateAssetMenu(fileName = "Action", menuName = "Clara Mundi/Actions/Action")]
  public class EntityAction : ScriptableObject
  {
    public string Id = "action";
    public string Name = "Action";
    public Sprite Sprite;

    public string Command = "/action";
    public string CommandShort = "/ac";

    public List<ActionArg> Args;
  }
}