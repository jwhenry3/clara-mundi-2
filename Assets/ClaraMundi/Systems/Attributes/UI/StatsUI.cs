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

        protected override void OnPlayerChange(Player _player)
        {
            if (entity != null)
            {
                player.Stats.OnStatsChange -= OnStatsChange;
                entity.NameChange -= OnNameChange;
            }
            base.OnPlayerChange(_player);
            if (entity == null) return;
            entity.NameChange += OnNameChange;
            player.Stats.OnStatsChange += OnStatsChange;
            OnNameChange(entity.entityName);
            OnStatsChange(player.Stats.ComputedStats);
        }

        private void OnNameChange(string playerName)
        {
            if (Name == null) return;
            Name.text = playerName;
        }

        private void OnStatsChange(ComputedStats stats)
        {
            if (player == null) return;
            Strength.text = DisplayNumber(stats.Strength);
            Dexterity.text = DisplayNumber(stats.Dexterity);
            Vitality.text = DisplayNumber(stats.Vitality);
            Agility.text = DisplayNumber(stats.Agility);
            Intelligence.text = DisplayNumber(stats.Intelligence);
            Mind.text = DisplayNumber(stats.Mind);
            
            var controller = player.Stats;
            
            PhysicalAttack.text = DisplayNumber(controller.Attributes.PhysicalAttack);
            PhysicalDefense.text = DisplayNumber(controller.Attributes.PhysicalDefense);
            PhysicalAccuracy.text = DisplayNumber(controller.Attributes.PhysicalAccuracy);
            PhysicalEvasion.text = DisplayNumber(controller.Attributes.PhysicalEvasion);
            
            MagicalAttack.text = DisplayNumber(controller.Attributes.MagicalAttack);
            Healing.text = DisplayNumber(controller.Attributes.Healing);
            MagicalDefense.text = DisplayNumber(controller.Attributes.MagicalDefense);
            MagicalAccuracy.text = DisplayNumber(controller.Attributes.MagicalAccuracy);
            MagicalEvasion.text = DisplayNumber(controller.Attributes.MagicalEvasion);
            
            SkillSpeed.text = DisplayPercent(controller.Attributes.PhysicalSpeed);
            SpellSpeed.text = DisplayPercent(controller.Attributes.MagicalSpeed);
        }

        private static string DisplayNumber(float value)
        {
            return Mathf.Floor(value) + "";
        }

        static string DisplayPercent(float value)
        {
            return Mathf.Floor(value * 100) + "%";
        }

    }
}