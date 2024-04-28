using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClaraMundi
{
  [CreateAssetMenu(fileName = "CharacterClassRepo", menuName = "Clara Mundi/Attributes/CharacterClassRepo")]
  [Serializable]
  public class CharacterClassRepo : ScriptableObject
  {

    public Dictionary<string, CharacterClassType> Classes;

    public CharacterClassType[] ClassList;

    public CharacterClassType GetClass(string classId)
    {
      if (!Classes.ContainsKey(classId.ToLower()))
        Classes[classId.ToLower()] = ClassList.First((c) => c.ClassId.ToLower() == classId.ToLower());

      return Classes[classId.ToLower()];
    }

  }
}