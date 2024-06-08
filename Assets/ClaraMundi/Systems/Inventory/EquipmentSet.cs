using System;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace ClaraMundi
{
  [Serializable]

  public class EquipmentSet
  {
    public int Main = -1;
    public int Sub = -1;
    public int Ranged = -1;
    public int Ammo = -1;
    public int Head = -1;
    public int Neck = -1;
    public int Body = -1;
    public int Hands = -1;
    public int Back = -1;
    public int Waist = -1;
    public int Legs = -1;
    public int Feet = -1;
    public int Ear1 = -1;
    public int Ear2 = -1;
    public int Ring1 = -1;
    public int Ring2 = -1;

    public EquipmentSet Clone()
    {
      var clone = new EquipmentSet();
      GetType().GetFields().ForEach((prop) => prop.SetValue(clone, prop.GetValue(this)));
      return clone;
    }

    public EquipmentSet Set(string field)
    {
      var property = GetType().GetField(field.ToLower().FirstCharacterToUpper());
      if (property == null)
      {
        Debug.Log("Cannot Find Item Slot: " + field.ToLower().FirstCharacterToUpper());
        return this;
      }
      property.SetValue(this, -1);
      return this;
    }
    public EquipmentSet Set(string field, int value)
    {
      var property = GetType().GetField(field.ToLower().FirstCharacterToUpper());
      if (property == null)
      {
        Debug.Log("Cannot Find Item Slot: " + field.ToLower().FirstCharacterToUpper());
        return this;
      }
      property.SetValue(this, value);
      return this;
    }
    public int Get(string field)
    {
      var property = GetType().GetField(field.ToLower().FirstCharacterToUpper());
      if (property == null)
      {
        Debug.Log("Cannot Find Item Slot: " + field.ToLower().FirstCharacterToUpper());
        return -1;
      }
      var value = property.GetValue(this);
      if (value != null)
      {
        return (int)value;
      }
      return -1;
    }
  }
}