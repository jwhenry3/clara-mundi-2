using System;
using System.Collections.Generic;

namespace ClaraMundi
{
  [Serializable]
  public class CharacterClass
  {
    public string classId = "adventurer";
    public int level = 1;
    public long exp = 0;
    public bool isCurrent = true;

    public EquipmentSet equipment = new();
  }
}