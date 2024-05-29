using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  [CreateAssetMenu(fileName = "Action", menuName = "Clara Mundi/Actions/Action")]
  public class EntityAction : ScriptableObject
  {
    public string Id = "action";
    public string Name = "Action";
    public Sprite Sprite;

    public string Command = "/action";
    public string CommandShort = "/ac";

    public string[] ArgNames;

    public bool MustHaveTarget;
    public bool TargetMustBePlayer;
    public bool TargetMustBeNPC;
    public bool CanBeUsedOnEnemy;
    public bool CanBeUsedOnAlly;
    public bool CanBeUsedOnSelf;
  }
}