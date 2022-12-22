using System;
using UnityEngine;

namespace ClaraMundi
{
    public enum EntityGroupClassification
    {
        Player,
        Vermin,
        Beast,
        Amorph,
        Aquan,
        Demon,
        Human,
        Undead,
        Avian,
        Beastman
    }

    public enum DetectionType
    {
        None,
        Sight,
        Sound
    }
    
    [CreateAssetMenu(fileName = "EntityType", menuName = "Clara Mundi/EntityType/EntityType")]
    [Serializable]
    public class EntityType : ScriptableObject
    {
        public string EntityTypeId = StringUtils.UniqueId();
        public string EntityTypeName = "";
        public EntityGroupClassification Classification = EntityGroupClassification.Player;
        public DetectionType DetectionType = DetectionType.None;
        public bool IsAggressive;
        public bool IsAware;
        public bool DoesLink;
        public bool IsElite;
        public LootItem[] Loot;
    }
}