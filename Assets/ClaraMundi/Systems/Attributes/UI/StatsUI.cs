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

        private void Awake()
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

        protected override void OnPlayerChange(Player _player)
        {
            if (entity != null)
            {
                player.Stats.OnChange -= OnStatsChange;
                entity.NameChange -= OnNameChange;
            }

            base.OnPlayerChange(_player);
            if (entity == null) return;
            entity.NameChange += OnNameChange;
            player.Stats.OnChange += OnStatsChange;
            OnNameChange(entity.entityName);
            OnStatsChange();
        }

        private void OnNameChange(string playerName)
        {
            if (Name == null) return;
            Name.text = playerName;
        }

        private void OnStatsChange()
        {
            if (player == null) return;
            var stats = player.Stats.ComputedStats;
            Strength.text = DisplayNumber(stats.Strength);
            Dexterity.text = DisplayNumber(stats.Dexterity);
            Vitality.text = DisplayNumber(stats.Vitality);
            Agility.text = DisplayNumber(stats.Agility);
            Intelligence.text = DisplayNumber(stats.Intelligence);
            Mind.text = DisplayNumber(stats.Mind);

            foreach (var kvp in player.Stats.Attributes)
            {
                if (!AttributeFields.ContainsKey(kvp.Key)) continue;
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