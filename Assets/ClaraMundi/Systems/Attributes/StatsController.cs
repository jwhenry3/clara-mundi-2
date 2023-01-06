using UnityEngine;
using System;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ClaraMundi
{
    public class StatsController : PlayerController
    {
        public event Action OnChange;

        [SyncVar(OnChange = nameof(OnComputedChange))]
        public ComputedStats ComputedStats = new();

        [SyncVar(OnChange = nameof(LevelChange))]
        public int Level = 1;

        [SyncVar(OnChange = nameof(ExpChange))]
        public long Experience = 0;

        [SyncVar(OnChange = nameof(ExpTilChange))]
        public long ExpTilNextLevel = 1000;

        [SyncVar(OnChange = nameof(EnergyChanged))]
        public Energies Energies = new();

        [ShowInInspector] Dictionary<StatType, List<StatValue>> StatModifications = new();
        [ShowInInspector] Dictionary<AttributeType, List<AttributeValue>> AttributeModifications = new();

        [ShowInInspector] public readonly Dictionary<StatType, StatValue> ModifiedStats = new();
        [ShowInInspector] public readonly Dictionary<AttributeType, AttributeValue> ModifiedAttributes = new();

        [ShowInInspector] [SyncObject(SendRate = 0.5f)] public readonly SyncDictionary<AttributeType, float> Attributes = new();
        bool hasLoadedStats;

        protected override void Awake()
        {
            base.Awake();
            Attributes.OnChange += OnAttributeChange;
        }

        private void OnDestroy()
        {
            Attributes.OnChange -= OnAttributeChange;
        }

        private void OnAttributeChange(SyncDictionaryOperation op, AttributeType key, float value, bool asServer)
        {
            OnChange?.Invoke();
        }

        private void LevelChange(int previous, int next, bool asServer)
        {
            OnChange?.Invoke();
        }

        private void ExpChange(long previous, long next, bool asServer)
        {
            OnChange?.Invoke();
        }

        private void ExpTilChange(long previous, long next, bool asServer)
        {
            OnChange?.Invoke();
        }

        public void UpdateStatModifications(StatValue[] values, bool add)
        {
            foreach (var mod in values)
            {
                if (add) AddStatModification(mod);
                if (!add) RemoveStatModification(mod);
            }
        }

        public void UpdateAttributeModifications(AttributeValue[] values, bool add)
        {
            foreach (var mod in values)
            {
                if (add) AddAttributeModification(mod);
                if (!add) RemoveAttributeModification(mod);
            }
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
        }

        public void AddAttributeModification(AttributeValue value)
        {
            if (!IsServer) return;
            if (!AttributeModifications.ContainsKey(value.Type))
                AttributeModifications[value.Type] = new();
            if (AttributeModifications[value.Type].Contains(value)) return;
            AttributeModifications[value.Type].Add(value);
            CalculateAttributeModificationsOf(value.Type);
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

            ModifiedAttributes[type] = newValue;
        }

        private Stats GetClassStats(CharacterClassType classType)
        {
            return new Stats
            {
                Strength = classType.StartingStats.Strength + classType.StatsPerLevel.Strength * Level,
                Vitality = classType.StartingStats.Vitality + classType.StatsPerLevel.Vitality * Level,
                Dexterity = classType.StartingStats.Dexterity + classType.StatsPerLevel.Dexterity * Level,
                Agility = classType.StartingStats.Agility + classType.StatsPerLevel.Agility * Level,
                Intelligence = classType.StartingStats.Intelligence + classType.StatsPerLevel.Intelligence * Level,
                Mind = classType.StartingStats.Mind + classType.StatsPerLevel.Mind * Level,
                Charisma = classType.StartingStats.Charisma + classType.StatsPerLevel.Charisma * Level,
            };
        }

        public void ComputeStats()
        {
            if (!IsServer) return;
            var classType = RepoManager.Instance.CharacterClassRepo.GetClass(player.Entity.CurrentClass.classId);
            var stats = GetClassStats(classType);
            ComputedStats = new ComputedStats
            {
                BaseStrength = stats.Strength,
                BaseVitality = stats.Vitality,
                BaseDexterity = stats.Dexterity,
                BaseAgility = stats.Agility,
                BaseIntelligence = stats.Intelligence,
                BaseMind = stats.Mind,
                BaseCharisma = stats.Charisma,
                Strength = GetModifiedStat(StatType.Strength, stats.Strength),
                Dexterity = GetModifiedStat(StatType.Dexterity, stats.Dexterity),
                Vitality = GetModifiedStat(StatType.Vitality, stats.Vitality),
                Agility = GetModifiedStat(StatType.Agility, stats.Agility),
                Intelligence = GetModifiedStat(StatType.Intelligence, stats.Intelligence),
                Mind = GetModifiedStat(StatType.Mind, stats.Mind),
                Charisma = GetModifiedStat(StatType.Strength, stats.Strength),
            };
            ComputeAttributes(classType);
            ComputeEnergies();
        }

        private void ComputeEnergies()
        {
            var energies = new Energies
            {
                DefaultHealth = Energies.DefaultHealth,
                DefaultMana = Energies.DefaultMana,
                DefaultStamina = Energies.DefaultStamina,
                MaxHealth = Math.Min(99999, Energies.DefaultHealth + (int)(2 * ComputedStats.Vitality)),
                Health = Math.Min(99999, Energies.DefaultMana + (int)(ComputedStats.Intelligence + ComputedStats.Mind)),
                MaxMana = Math.Min(99999,
                    Energies.DefaultStamina + (int)(ComputedStats.Vitality + ComputedStats.Dexterity +
                                                    ComputedStats.Agility + ComputedStats.Strength)),
                Mana = Mathf.Min(Energies.Health, Energies.MaxHealth),
                MaxStamina = Mathf.Min(Energies.Mana, Energies.MaxMana),
                Stamina = Mathf.Min(Energies.Stamina, Energies.MaxStamina)
            };
            if (!hasLoadedStats)
            {
                energies.Health = energies.MaxHealth;
                energies.Mana = energies.MaxMana;
                energies.Stamina = energies.MaxStamina;
                hasLoadedStats = true;
            }

            Energies = energies;
        }

        private void ComputeAttributes(CharacterClassType classType)
        {
            foreach (var kvp in classType.CalculationDict)
                Attributes[kvp.Key] = GetModifiedAttribute(kvp.Key, classType);
        }

        private float GetModifiedAttribute(AttributeType type, CharacterClassType classType)
        {
            var initial = classType != null ? classType.GetBaseValueFor(type, ComputedStats) : 1;
            return !ModifiedAttributes.ContainsKey(type) ? initial : ModifiedAttributes[type].GetModifiedValue(initial);
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
            OnChange?.Invoke();
        }

        private void OnComputedChange(ComputedStats oldValue, ComputedStats newValue, bool asServer)
        {
            OnChange?.Invoke();
        }


        public void AddExperience(int amount)
        {
            if (!IsServer) return;

            Experience += amount;
            ExpTilNextLevel -= amount;
            if (ExpTilNextLevel > 0) return;

            Level += 1;
            // read from a list of exp per level
            ExpTilNextLevel = 1000;
            OnLevelChange();
        }
    }
}