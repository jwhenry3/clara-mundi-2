using System;
using System.Collections.Generic;

namespace ClaraMundi
{
  [Serializable]
  public class Character
  {
    public string accountId;
    public string name;
    public string gender;
    public string race;
    public string area;
    public float position_x;
    public float position_y;
    public float position_z;
    public float rotation;
    public string classId;
    public int level;
    public long exp;

    public long lastConnected;
    public long lastDisconnected;

    public bool hasConnectedBefore;

    public List<CharacterClass> characterClasses;
  }
}