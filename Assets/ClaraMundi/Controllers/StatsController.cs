using UnityEngine;
using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Object.Synchronizing.SecretMenu;
using Sirenix.OdinInspector;

namespace ClaraMundi
{
    public class StatsController : PlayerController
    {
        public event Action<ComputedStats> OnStatsChange;
        public event Action OnChange;
        public event Action OnEnergyChange;
        public Stats Stats = new();
        [SyncVar(OnChange = "OnComputedChange")]
        public ComputedStats ComputedStats = new();

        public int Level = 1;
        public int Experience = 0;
        public int ExpTilNextLevel = 1000;

        public Stats BaseStats = new();
        [SyncVar(OnChange="EnergyChanged")]
        public Energies Energies = new();

        public Attributes Attributes;

        [ShowInInspector]
        Dictionary<StatType, List<StatValue>> StatModifications = new();
        [ShowInInspector]
        Dictionary<AttributeType, List<AttributeValue>> AttributeModifications = new();

        [ShowInInspector]
        [SyncObject]
        public readonly SyncDictionary<StatType, StatValue> ModifiedStats = new();
        [ShowInInspector]
        [SyncObject]
        public readonly SyncDictionary<AttributeType, AttributeValue> ModifiedAttributes = new();

        bool hasLoadedStats;


        protected override void Awake()
        {
            Attributes = new Attributes()
            {
                stats = this
            };
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            ComputeStats();
        }

        public void UpdateStatModification(StatValue value, bool add)
        {
            if (add) AddStatModification(value);
            if (!add) RemoveStatModification(value);
        }
        public void UpdateAttributeModification(AttributeValue value, bool add)
        {
            if (add) AddAttributeModification(value);
            if (!add) RemoveAttributeModification(value);
        }
        public void AddStatModification(StatValue value)
        {
            if (!IsServer) return;
            if (!StatModifications.ContainsKey(value.Type))
                StatModifications[value.Type] = new();
            if (StatModifications[value.Type].Contains(value)) return;
            StatModifications[value.Type].Add(value);
            CalculateStatModificationsOf(value.Type);
        }

        public void RemoveStatModification(StatValue value)
        {
            if (!IsServer) return;
            if (StatModifications.ContainsKey(value.Type))
            {
                if (StatModifications[value.Type].Contains(value))
                    StatModifications[value.Type].Remove(value);
            }
            CalculateStatModificationsOf(value.Type);
        }

        private void CalculateStatModificationsOf(StatType type)
        {
            if (!IsServer) return;
            var newValue = new StatValue
            {
                Type = type
            };
            if (StatModifications.ContainsKey(type))
            {
                foreach (var sValue in StatModifications[type])
                {
                    newValue.Amount += sValue.Amount;
                    newValue.Percent += sValue.Percent;
                }
            }
            ModifiedStats[type] = newValue;
            ComputeStats();
        }

        public void AddAttributeModification(AttributeValue value)
        {
            if (!IsServer) return;
            if (!AttributeModifications.ContainsKey(value.Type))
                AttributeModifications[value.Type] = new();
            if (AttributeModifications[value.Type].Contains(value)) return;
            AttributeModifications[value.Type].Add(value);
            CalculateAttributeModificationsOf(value.Type);
            ComputeStats();
        }

        public void RemoveAttributeModification(AttributeValue value)
        {
            if (!IsServer) return;
            if (AttributeModifications.ContainsKey(value.Type))
            {
                if (AttributeModifications[value.Type].Contains(value))
                    AttributeModifications[value.Type].Remove(value);
            }
            CalculateAttributeModificationsOf(value.Type);
        }

        private void CalculateAttributeModificationsOf(AttributeType type)
        {
            if (!IsServer) return;
            var newValue = new AttributeValue
            {
                Type = type
            };
            if (AttributeModifications.ContainsKey(type))
            {
                foreach (var sValue in AttributeModifications[type])
                {
                    newValue.Amount += sValue.Amount;
                    newValue.Percent += sValue.Percent;
                }
            }
            Attributes.Modifications[type] = newValue;
        }

        private void ComputeStats()
        {
            if (!IsServer) return;
            Stats = new Stats
            {
                Strength = BaseStats.Strength  * Level ,
                Vitality = BaseStats.Vitality * Level,
                Dexterity = BaseStats.Dexterity * Level,
                Agility = BaseStats.Agility  * Level,
                Intelligence = BaseStats.Intelligence  * Level,
                Mind = BaseStats.Mind  * Level,
                Charisma = BaseStats.Charisma * Level,
            };
            ComputedStats = new ComputedStats
            {
                BaseStrength = Stats.Strength,
                BaseVitality = Stats.Vitality,
                BaseDexterity = Stats.Dexterity,
                BaseAgility = Stats.Agility,
                BaseIntelligence = Stats.Intelligence,
                BaseMind = Stats.Mind,
                BaseCharisma = Stats.Charisma,
                Strength = GetModifiedStat(StatType.Strength, Stats.Strength),
                Dexterity = GetModifiedStat(StatType.Dexterity, Stats.Dexterity),
                Vitality = GetModifiedStat(StatType.Vitality, Stats.Vitality),
                Agility = GetModifiedStat(StatType.Agility, Stats.Agility),
                Intelligence = GetModifiedStat(StatType.Intelligence, Stats.Intelligence),
                Mind = GetModifiedStat(StatType.Mind, Stats.Mind),
                Charisma = GetModifiedStat(StatType.Strength, Stats.Strength),
            };
            Energies.MaxHealth = Math.Min(99999, Energies.DefaultHealth + (int)(2 * ComputedStats.Vitality));
            Energies.MaxMana = Math.Min(99999, Energies.DefaultMana + (int)(ComputedStats.Intelligence + ComputedStats.Mind));
            // roughly averaged between 4 physical stats
            Energies.MaxStamina = Math.Min(99999, Energies.DefaultStamina + (int)(ComputedStats.Vitality + ComputedStats.Dexterity + ComputedStats.Agility + ComputedStats.Strength));
            Energies.Health = Mathf.Min(Energies.Health, Energies.MaxHealth);
            Energies.Mana = Mathf.Min(Energies.Mana, Energies.MaxMana);
            Energies.Stamina = Mathf.Min(Energies.Stamina, Energies.MaxStamina);
            if (!hasLoadedStats)
            {
                Energies.Health = Energies.MaxHealth;
                Energies.Mana = Energies.MaxMana;
                Energies.Stamina = Energies.MaxStamina;
                hasLoadedStats = true;
            }
            Energies.Dirty();
        }

        private int GetModifiedStat(StatType type, float initial)
        {
            var value = ModifiedStats.ContainsKey(type) ? ModifiedStats[type].GetModifiedValue(initial) : initial;
            return Math.Min(9999, Mathf.FloorToInt(value));
        }


        private void OnLevelChange()
        {
            if (!IsServer) return;
            ComputeStats();
        }

        private void EnergyChanged(Energies previous, Energies next, bool asServer)
        {
            OnEnergyChange?.Invoke();
        }

        private void OnComputedChange(ComputedStats oldValue, ComputedStats newValue, bool asServer)
        {
            OnStatsChange?.Invoke(newValue);
            OnChange?.Invoke();
        }

        public void AddExperience(int amount)
        {
            if (!IsServer) return;

            Experience += amount;
            ExpTilNextLevel-= amount;
            if (ExpTilNextLevel > 0) return;
            
            Level += 1;
            // read from a list of exp per level
            ExpTilNextLevel = 1000;
            OnLevelChange();
        }
    }
}