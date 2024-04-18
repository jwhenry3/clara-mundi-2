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
        if (player.Entity.entityName.Value == PlayerManager.Instance.LocalPlayer.Character.name)
          transform.SetAsFirstSibling();
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
        if (HealthBar != null)
          HealthBar.fillAmount = 0;
        if (ManaBar != null)
          ManaBar.fillAmount = 0;
        if (HealthText != null)
          HealthText.text = "??";
        if (ManaText != null)
          ManaText.text = "??";
        return;
      }
      PlayerLevel.text = "LV " + player.Stats.Level.Value;
      // convert a value to a float to retain the decimal value
      if (HealthBar != null)
        HealthBar.fillAmount = player.Stats.Energies.Value.Health / (player.Stats.Energies.Value.MaxHealth * 1f);
      if (ManaBar != null)
        ManaBar.fillAmount = player.Stats.Energies.Value.Mana / (player.Stats.Energies.Value.MaxMana * 1f);
      if (HealthText != null)
        HealthText.text = player.Stats.Energies.Value.Health + "";
      if (ManaText != null)
        ManaText.text = player.Stats.Energies.Value.Mana + "";
    }
  }
}