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

    public Dictionary<string, CharacterClassType> Classes = new();

    public CharacterClassType[] ClassList;

    public CharacterClassType GetClass(string classId)
    {
      if (String.IsNullOrEmpty(classId)) return null;
      if (!Classes.ContainsKey(classId.ToLower()))
        Classes[classId.ToLower()] = ClassList.First((c) => c.ClassId.ToLower() == classId.ToLower());

      return Classes[classId.ToLower()];
    }

  }
}