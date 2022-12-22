using System;
using UnityEditor;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "Zone", menuName = "Clara Mundi/World/Zone")]
    [Serializable]
    public class Zone : ScriptableObject
    {
        public string Name;
        public string Key;
        public SceneAsset Scene;
        [HideInInspector]
        public Region Region;
        public Zone[] AdjacentZones;
    }
}