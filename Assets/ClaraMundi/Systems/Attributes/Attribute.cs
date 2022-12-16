using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ClaraMundi
{
    
    public class Attributes
    {
        public StatsController stats;
        public ComputedStats Stats => stats.ComputedStats;
        public SyncDictionary<AttributeType, AttributeValue> Modifications => stats.ModifiedAttributes;

        public EffectType PotencyType = EffectType.Physical;
        public StatType PrimaryStat = StatType.Strength;
        public StatType SecondaryStat = StatType.Strength;
        public float PhysicalAttack => GetModifiedValue(AttributeType.PhysicalAttack, GetPhysicalPotency());
        public float PhysicalDefense => GetModifiedValue(AttributeType.PhysicalDefense, Stats.Vitality);
        public float PhysicalAccuracy => GetModifiedValue(AttributeType.PhysicalAccuracy, Stats.Dexterity + Stats.Agility * 0.25f);
        public float PhysicalEvasion => GetModifiedValue(AttributeType.PhysicalEvasion, Stats.Agility + Stats.Dexterity * 0.25f);
        public float MagicalAttack => GetModifiedValue(AttributeType.MagicalAttack, GetMagicalPotency());
        public float MagicalDefense => GetModifiedValue(AttributeType.MagicalDefense, Stats.Mind);
        public float MagicalAccuracy => GetModifiedValue(AttributeType.MagicalAccuracy, Stats.Intelligence * 0.5f + Stats.Mind * 0.5f);
        public float MagicalEvasion => GetModifiedValue(AttributeType.MagicalEvasion, Stats.Intelligence * 0.5f + Stats.Mind * 0.5f);

        public float PhysicalSpeed => Mathf.Min(3, GetModifiedValue(AttributeType.PhysicalSpeed, 1));
        public float MagicalSpeed => Mathf.Min(3, GetModifiedValue(AttributeType.MagicalSpeed, 1));
        public float Healing => Mathf.Min(3, GetModifiedValue(AttributeType.MagicalSpeed, Stats.Mind * 1.5f));


        private float GetModifiedValue(AttributeType type, float initial)
        {
            return !Modifications.ContainsKey(type) ? initial : Modifications[type].GetModifiedValue(initial);
        }

        private float GetPhysicalPotency()
        {
            if (PotencyType == EffectType.Physical)
                return GetStat(PrimaryStat) * 1.5f + (GetStat(SecondaryStat));
            else
                return Stats.Strength + (Stats.Dexterity * 0.75f); // less efficiency if current weapon is not geared for physical damage
        }

        private float GetMagicalPotency()
        {
            if (PotencyType == EffectType.Magical)
                return GetStat(PrimaryStat) * 1.5f + (GetStat(SecondaryStat));
            else
                return Stats.Intelligence + (Stats.Mind * 0.75f); // less efficiency if current weapon is not geared for magical damage
        }

        private float GetStat(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => Stats.Strength,
                StatType.Dexterity => Stats.Dexterity,
                StatType.Vitality => Stats.Vitality,
                StatType.Agility => Stats.Agility,
                StatType.Intelligence => Stats.Intelligence,
                StatType.Mind => Stats.Mind,
                StatType.Charisma => Stats.Charisma,
                _ => 0
            };
        }
    }
}