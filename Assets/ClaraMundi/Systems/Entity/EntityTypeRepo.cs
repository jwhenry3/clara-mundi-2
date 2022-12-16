using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "EntityTypeRepo", menuName = "Clara Mundi/EntityType/EntityTypeRepo")]
    [Serializable]
    public class EntityTypeRepo : ScriptableObject
    {
        [HideInInspector]
        public Dictionary<string, EntityType> EntityTypes;

        public EntityType[] EntityTypeList;

        
        public void OnEnable()
        {
            EntityTypes = new();
            if (EntityTypeList == null) return;
            foreach (var EntityGroup in EntityTypeList)
            {
                EntityTypes.Add(EntityGroup.EntityTypeId, EntityGroup);
            }
        }

    }
}