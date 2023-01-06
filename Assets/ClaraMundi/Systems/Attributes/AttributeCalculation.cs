using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "AttributeCalculation", menuName = "Clara Mundi/Attributes/AttributeCalculation")]
    [Serializable]
    public class AttributeCalculation : ScriptableObject
    {
        public AttributeType ImpactingAttribute = AttributeType.PhysicalAttack;
        public List<StatValue> AffectingStats;

        public float GetValue(ComputedStats stats)
        {
            float finalValue = 0;
            foreach (var value in AffectingStats)
            {
                switch (value.Type)
                {
                    case StatType.Strength:
                        finalValue += GetModifiedValue(stats.Strength, value);
                        break;
                    case StatType.Dexterity:
                        finalValue += GetModifiedValue(stats.Dexterity, value);
                        break;
                    case StatType.Vitality:
                        finalValue += GetModifiedValue(stats.Vitality, value);
                        break;
                    case StatType.Agility:
                        finalValue += GetModifiedValue(stats.Agility, value);
                        break;
                    case StatType.Intelligence:
                        finalValue += GetModifiedValue(stats.Intelligence, value);
                        break;
                    case StatType.Mind:
                        finalValue += GetModifiedValue(stats.Mind, value);
                        break;
                    case StatType.Charisma:
                        finalValue += GetModifiedValue(stats.Charisma, value);
                        break;
                }
            }

            return finalValue;
        }

        private float GetModifiedValue(float initial, StatValue stat)
        {
            if (stat.Percent != 0)
                return initial * (stat.Percent / 100);
            return initial + stat.Amount;
        }
    }
}