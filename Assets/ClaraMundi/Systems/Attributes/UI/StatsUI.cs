using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace ClaraMundi
{
  public class StatsUI : PlayerUI
  {
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Experience;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Strength;
    public TextMeshProUGUI Dexterity;
    public TextMeshProUGUI Vitality;
    public TextMeshProUGUI Agility;
    public TextMeshProUGUI Intelligence;
    public TextMeshProUGUI Mind;
    public TextMeshProUGUI PhysicalAttack;
    public TextMeshProUGUI PhysicalDefense;
    public TextMeshProUGUI PhysicalAccuracy;
    public TextMeshProUGUI PhysicalEvasion;
    public TextMeshProUGUI SkillSpeed;
    public TextMeshProUGUI MagicalAttack;
    public TextMeshProUGUI Healing;
    public TextMeshProUGUI MagicalDefense;
    public TextMeshProUGUI MagicalAccuracy;
    public TextMeshProUGUI MagicalEvasion;
    public TextMeshProUGUI SpellSpeed;

    private readonly Dictionary<AttributeType, TextMeshProUGUI> AttributeFields = new();
    public AttributeType[] PercentAttributes = { AttributeType.MagicalSpeed, AttributeType.PhysicalSpeed };


    protected override void OnPlayerChange(Player _player)
    {
      if (entity != null)
      {
        player.Stats.OnChange -= OnStatsChange;
        entity.entityName.OnChange -= OnNameChange;
      }

      base.OnPlayerChange(_player);
      if (entity == null) return;
      entity.entityName.OnChange += OnNameChange;
      player.Stats.OnChange += OnStatsChange;
      player.Stats.Energies.OnChange += OnEnergyChange;
      OnNameChange("", entity.entityName.Value, false);
      OnStatsChange();
      OnEnergyChange(default, player.Stats.Energies.Value, false);
    }

    private void OnEnergyChange(Energies prev, Energies next, bool asServer)
    {
      if (Health != null)
        Health.text = next.Health + " / " + next.MaxHealth;
      if (Mana != null)
        Mana.text = next.Mana + " / " + next.MaxMana;
    }

    private void OnNameChange(string prev, string playerName, bool asServer)
    {
      if (Name == null) return;
      Name.text = playerName;
    }

    private void OnStatsChange()
    {
      if (player == null) return;
      var stats = player.Stats.ComputedStats.Value;
      if (Level != null)
        Level.text = player.Stats.Level.Value + "";
      if (Experience != null)
        Experience.text = player.Stats.ExpTilNextLevel.Value + "";
      if (Strength != null)
        Strength.text = DisplayNumber(stats.Strength);
      if (Dexterity != null)
        Dexterity.text = DisplayNumber(stats.Dexterity);
      if (Vitality != null)
        Vitality.text = DisplayNumber(stats.Vitality);
      if (Agility != null)
        Agility.text = DisplayNumber(stats.Agility);
      if (Intelligence != null)
        Intelligence.text = DisplayNumber(stats.Intelligence);
      if (Mind != null)
        Mind.text = DisplayNumber(stats.Mind);

      if (AttributeFields.Count == 0)
      {
        AttributeFields[AttributeType.Healing] = Healing;
        AttributeFields[AttributeType.PhysicalDefense] = PhysicalDefense;
        AttributeFields[AttributeType.PhysicalAttack] = PhysicalAttack;
        AttributeFields[AttributeType.PhysicalAccuracy] = PhysicalAccuracy;
        AttributeFields[AttributeType.PhysicalEvasion] = PhysicalEvasion;
        AttributeFields[AttributeType.PhysicalSpeed] = SkillSpeed;
        AttributeFields[AttributeType.MagicalAttack] = MagicalAttack;
        AttributeFields[AttributeType.MagicalAccuracy] = MagicalAccuracy;
        AttributeFields[AttributeType.MagicalEvasion] = MagicalEvasion;
        AttributeFields[AttributeType.MagicalDefense] = MagicalDefense;
        AttributeFields[AttributeType.MagicalSpeed] = SpellSpeed;
      }

      foreach (var kvp in player.Stats.Attributes)
      {
        if (!AttributeFields.ContainsKey(kvp.Key)) continue;
        if (AttributeFields[kvp.Key] == null) continue;
        AttributeFields[kvp.Key].text = DisplayNumber(kvp.Value, PercentAttributes.Contains(kvp.Key));
      }
    }

    private static string DisplayNumber(float value, bool percent = false)
    {
      if (percent) return Mathf.Floor(value * 100) + "%";
      return Mathf.Floor(value) + "";
    }
  }
}