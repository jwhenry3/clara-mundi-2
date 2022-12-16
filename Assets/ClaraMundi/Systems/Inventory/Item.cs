using System;
using Sirenix.OdinInspector;
using UnityEngine;
namespace ClaraMundi
{
    public enum ItemType
    {
        Generic,
        Ingredient,
        Consumable,
        KeyItem,
        Armor,
        Weapon
    }
    [CreateAssetMenu(fileName = "Item", menuName = "Clara Mundi/Inventory/Item")]
    [Serializable]
    public class Item : ScriptableObject
    {
        [BoxGroup("Identity")]
        public string ItemId = "";
        [BoxGroup("Identity")]
        public string Name = "";
        [BoxGroup("Identity")]
        public string Description = "";
        [BoxGroup("Classification")]
        public ItemType Type;
        [BoxGroup("UI")]
        public Sprite Icon;
        [BoxGroup("UI")]
        public Sprite Background;
        
        [TabGroup("Rules")]
        public bool Stackable;
        [TabGroup("Rules")]
        public bool Unique;
        [TabGroup("Rules")]
        public bool Untradeable;
        [TabGroup("Rules")]
        public bool Droppable;
        [TabGroup("Rules")]
        public bool Usable;
        [TabGroup("Rules")]
        public bool Equippable;
        [TabGroup("Rules")]
        public string EquipmentSlot;

        [TabGroup("Equipment")]
        public ElementType ElementType = ElementType.Neutral;
        [TabGroup("Equipment")]
        public EffectType EffectType = EffectType.Physical;
        [TabGroup("Equipment")]
        public StatType PrimaryStat = StatType.Strength;
        [TabGroup("Equipment")]
        public StatType SecondaryStat = StatType.Dexterity;
        [TabGroup("Equipment")]
        public GameObject Prefab;
        [TabGroup("Equipment")]
        public string WieldedBone;
        [TabGroup("Equipment")]
        public Vector3 WieldedOffset = Vector3.zero;
        [TabGroup("Equipment")]
        public Vector3 WieldedRotation = Vector3.zero;
        [TabGroup("Equipment")]
        public string SheathedBone;
        [TabGroup("Equipment")]
        public Vector3 SheathedOffset = Vector3.zero;
        [TabGroup("Equipment")]
        public Vector3 SheathedRotation = Vector3.zero;
        [TabGroup("Equipment")]
        public StatValue[] StatModifications;
        [TabGroup("Equipment")]
        public AttributeValue[] AttributeModifications;
    }
}