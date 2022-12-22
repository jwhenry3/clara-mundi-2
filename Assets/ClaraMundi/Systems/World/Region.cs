using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "Region", menuName = "Clara Mundi/World/Region")]
    [Serializable]
    public class Region : ScriptableObject
    {
        public Dictionary<string, Zone> ZoneDict = new();
        public string Name;
        public string Key;
        public Zone[] Zones;
    }
}