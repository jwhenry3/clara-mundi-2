using System;
using System.Collections.Generic;

namespace ClaraMundi
{
    [Serializable]
    public class CharacterClass
    {
        public string classId;
        public int level;
        public long exp;
        public bool isCurrent;

        public List<CharacterEquipment> equipment = new();
    }
}