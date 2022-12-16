using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
    public class PartyMemberUI : MonoBehaviour
    {
        public Player player { get; private set; }
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI PlayerLevel;
        public ProceduralImage HealthBar;
        public ProceduralImage ManaBar;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI ManaText;
        
        public void SetPartyMember(string playerId)
        {
            if (player != null)
            {
                player.Stats.OnChange -= OnChange;
            } 
            player = null;
            if (!string.IsNullOrEmpty(playerId) && PlayerManager.Instance.Players.ContainsKey(playerId))
            {
                player = PlayerManager.Instance.Players[playerId];
                player.Stats.OnChange += OnChange;
            }

            if (player == null)
            {
                Destroy(gameObject);
                return;
            }

            PlayerName.text = player.Entity.entityName;
            OnChange();
        }

        private void OnChange()
        {
            PlayerLevel.text = "LV " + player.Stats.Level;
            // convert a value to a float to retain the decimal value
            HealthBar.fillAmount = (player.Stats.Energies.Health / (player.Stats.Energies.MaxHealth * 1f));
            ManaBar.fillAmount = (player.Stats.Energies.Mana / (player.Stats.Energies.MaxMana * 1f));
            HealthText.text = player.Stats.Energies.Health + "";
            ManaText.text = player.Stats.Energies.Mana + "";
        }
    }
}