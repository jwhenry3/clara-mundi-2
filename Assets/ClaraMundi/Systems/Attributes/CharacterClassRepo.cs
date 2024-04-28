using System;
using System.Collections.Generic;
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
      return Classes.ContainsKey(classId.ToLower()) ? Classes[classId.ToLower()] : null;
    }

    public void OnEnable()
    {
      Classes = new();
      if (ClassList == null) return;
      foreach (var classType in ClassList)
      {
        Classes.Add(classType.ClassId.ToLower(), classType);
      }
    }
  }
}