using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "CharacterClassType", menuName = "Clara Mundi/Attributes/CharacterClassType")]
    [Serializable]
    public class CharacterClassType : ScriptableObject
    {
        public string ClassId;
        public string ClassName;
        public Stats StartingStats;

        public Stats StatsPerLevel;

        public List<AttributeCalculation> AttributeCalculations;

        public Dictionary<AttributeType, AttributeCalculation> CalculationDict;

        public float GetBaseValueFor(AttributeType attribute, ComputedStats stats)
        {
            if (!CalculationDict.ContainsKey(attribute)) return 0;
            return CalculationDict[attribute].GetValue(stats);
        }

        private void OnEnable()
        {
            if (AttributeCalculations.Count == 0) return;
            CalculationDict = new();
            foreach (var calculation in AttributeCalculations)
                CalculationDict[calculation.ImpactingAttribute] = calculation;
        }
    }
}