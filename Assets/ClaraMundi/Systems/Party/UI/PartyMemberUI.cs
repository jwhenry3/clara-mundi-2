using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
    public class PartyMemberUI : MonoBehaviour
    {
        public Player player { get; private set; }
        public string playerName;
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI PlayerLevel;
        public ProceduralImage HealthBar;
        public ProceduralImage ManaBar;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI ManaText;
        
        public void SetPartyMember(string characterName)
        {
            playerName = characterName;
            if (player != null)
            {
                player.Stats.OnChange -= OnChange;
            } 
            player = null;
            if (!string.IsNullOrEmpty(playerName) && PlayerManager.Instance.PlayersByName.ContainsKey(playerName))
            {
                player = PlayerManager.Instance.PlayersByName[playerName];
                player.Stats.OnChange += OnChange;
            }

            if (string.IsNullOrEmpty(playerName))
            {
                Destroy(gameObject);
                return;
            }

            PlayerName.text = playerName;
            OnChange();
        }

        private void OnDestroy()
        {
            if (player != null)
            {
                player.Stats.OnChange -= OnChange;
            } 
        }

        private void OnChange()
        {
            if (player == null)
            {
                PlayerLevel.text = "";
                // convert a value to a float to retain the decimal value
                HealthBar.fillAmount = 0;
                ManaBar.fillAmount = 0;
                HealthText.text = "??";
                ManaText.text = "??";
                return;
            }
            PlayerLevel.text = "LV " + player.Stats.Level.Value;
            // convert a value to a float to retain the decimal value
            HealthBar.fillAmount = player.Stats.Energies.Value.Health / (player.Stats.Energies.Value.MaxHealth * 1f);
            ManaBar.fillAmount = player.Stats.Energies.Value.Mana / (player.Stats.Energies.Value.MaxMana * 1f);
            HealthText.text = player.Stats.Energies.Value.Health + "";
            ManaText.text = player.Stats.Energies.Value.Mana + "";
        }
    }
}