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
    public readonly SyncVar<ComputedStats> ComputedStats = new(new ComputedStats());
    public readonly SyncVar<int> Level = new(1);
    public readonly SyncVar<long> Experience = new(0);
    public readonly SyncVar<long> ExpTilNextLevel = new(1000);
    public readonly SyncVar<Energies> Energies = new(new Energies());

    [ShowInInspector] private Dictionary<StatType, List<StatValue>> StatModifications = new();
    [ShowInInspector] private Dictionary<AttributeType, List<AttributeValue>> AttributeModifications = new();

    [ShowInInspector] public readonly Dictionary<StatType, StatValue> ModifiedStats = new();
    [ShowInInspector] public readonly Dictionary<AttributeType, AttributeValue> ModifiedAttributes = new();

    [ShowInInspector] public readonly SyncDictionary<AttributeType, float> Attributes = new();
    private bool hasLoadedStats;

    private void OnEnable()
    {
      ComputedStats.OnChange += OnComputedChange;
      Level.OnChange += LevelChange;
      Experience.OnChange += ExpChange;
      ExpTilNextLevel.OnChange += ExpTilChange;
      Energies.OnChange += EnergyChanged;
      OnChange?.Invoke();
    }

    public override void OnStartServer()
    {
      base.OnStartServer();
      ComputeStats();
    }

    private void OnDisable()
    {
      ComputedStats.OnChange -= OnComputedChange;
      Level.OnChange -= LevelChange;
      Experience.OnChange -= ExpChange;
      ExpTilNextLevel.OnChange -= ExpTilChange;
      Energies.OnChange -= EnergyChanged;
    }

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
      Debug.Log("Attributes Change");
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
      if (!IsServerStarted) return;
      if (!StatModifications.ContainsKey(value.Type))
        StatModifications[value.Type] = new();
      if (StatModifications[value.Type].Contains(value)) return;
      StatModifications[value.Type].Add(value);
      CalculateStatModificationsOf(value.Type);
    }

    public void RemoveStatModification(StatValue value)
    {
      if (!IsServerStarted) return;
      if (StatModifications.ContainsKey(value.Type))
      {
        if (StatModifications[value.Type].Contains(value))
          StatModifications[value.Type].Remove(value);
      }

      CalculateStatModificationsOf(value.Type);
    }

    private void CalculateStatModificationsOf(StatType type)
    {
      if (!IsServerStarted) return;
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
      if (!IsServerStarted) return;
      if (!AttributeModifications.ContainsKey(value.Type))
        AttributeModifications[value.Type] = new();
      if (AttributeModifications[value.Type].Contains(value)) return;
      AttributeModifications[value.Type].Add(value);
      CalculateAttributeModificationsOf(value.Type);
    }

    public void RemoveAttributeModification(AttributeValue value)
    {
      if (!IsServerStarted) return;
      if (AttributeModifications.ContainsKey(value.Type))
      {
        if (AttributeModifications[value.Type].Contains(value))
          AttributeModifications[value.Type].Remove(value);
      }

      CalculateAttributeModificationsOf(value.Type);
    }

    private void CalculateAttributeModificationsOf(AttributeType type)
    {
      if (!IsServerStarted) return;
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
        Strength = classType.StartingStats.Strength + classType.StatsPerLevel.Strength * Level.Value,
        Vitality = classType.StartingStats.Vitality + classType.StatsPerLevel.Vitality * Level.Value,
        Dexterity = classType.StartingStats.Dexterity + classType.StatsPerLevel.Dexterity * Level.Value,
        Agility = classType.StartingStats.Agility + classType.StatsPerLevel.Agility * Level.Value,
        Intelligence = classType.StartingStats.Intelligence + classType.StatsPerLevel.Intelligence * Level.Value,
        Mind = classType.StartingStats.Mind + classType.StatsPerLevel.Mind * Level.Value,
        Charisma = classType.StartingStats.Charisma + classType.StatsPerLevel.Charisma * Level.Value,
      };
    }

    public void ComputeStats()
    {
      if (!IsServerStarted) return;
      var classType = RepoManager.Instance.CharacterClassRepo.GetClass(player.Entity.CurrentClass.classId);
      if (classType == null)
      {
        Debug.LogWarning("Cannot compute class stats for " + player.Entity.CurrentClass.classId);
        return;
      }
      var stats = GetClassStats(classType);
      ComputedStats.Value = new ComputedStats
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
      var stats = ComputedStats.Value;
      var energies = new Energies
      {
        DefaultHealth = Energies.Value.DefaultHealth,
        DefaultMana = Energies.Value.DefaultMana,
        DefaultStamina = Energies.Value.DefaultStamina,
        MaxHealth = Math.Min(99999, Energies.Value.DefaultHealth + (int)(2 * stats.Vitality)),
        Health = Math.Min(99999, Energies.Value.DefaultMana + (int)(stats.Intelligence + stats.Mind)),
        MaxMana = Math.Min(99999,
              Energies.Value.DefaultStamina + (int)(stats.Vitality + stats.Dexterity +
                                              stats.Agility + stats.Strength)),
        Mana = Mathf.Min(Energies.Value.Health, Energies.Value.MaxHealth),
        MaxStamina = Mathf.Min(Energies.Value.Mana, Energies.Value.MaxMana),
        Stamina = Mathf.Min(Energies.Value.Stamina, Energies.Value.MaxStamina)
      };
      if (!hasLoadedStats)
      {
        energies.Health = energies.MaxHealth;
        energies.Mana = energies.MaxMana;
        energies.Stamina = energies.MaxStamina;
        hasLoadedStats = true;
      }

      Energies.Value = energies;
    }

    private void ComputeAttributes(CharacterClassType classType)
    {
      foreach (var kvp in classType.CalculationDict)
      {
        Attributes[kvp.Key] = GetModifiedAttribute(kvp.Key, classType);
      }
    }

    private float GetModifiedAttribute(AttributeType type, CharacterClassType classType)
    {
      var initial = classType != null ? classType.GetBaseValueFor(type, ComputedStats.Value) : 1;
      return !ModifiedAttributes.ContainsKey(type) ? initial : ModifiedAttributes[type].GetModifiedValue(initial);
    }

    private int GetModifiedStat(StatType type, float initial)
    {
      var value = ModifiedStats.ContainsKey(type) ? ModifiedStats[type].GetModifiedValue(initial) : initial;
      return Math.Min(9999, Mathf.FloorToInt(value));
    }


    private void OnLevelChange()
    {
      if (!IsServerStarted) return;
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
      if (!IsServerStarted) return;

      Experience.Value += amount;
      ExpTilNextLevel.Value -= amount;
      if (ExpTilNextLevel.Value > 0) return;

      Level.Value += 1;
      // read from a list of exp per level
      ExpTilNextLevel.Value = 1000;
      OnLevelChange();
    }
  }
}