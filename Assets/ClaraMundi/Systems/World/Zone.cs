using System;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "Zone", menuName = "Clara Mundi/World/Zone")]
    [Serializable]
    public class Zone : ScriptableObject
    {
        public string Name;
        public string Key;
        [HideInInspector]
        public Region Region;
        public Zone[] AdjacentZones;
    }
}