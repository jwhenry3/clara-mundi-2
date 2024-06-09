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

    public ButtonUI button;
    void OnEnable()
    {
      button = button ?? GetComponent<ButtonUI>();
    }
    public void SetPartyMember(string characterName)
    {
      playerName = characterName;
      if (player != null)
      {
        player.Stats.OnChange -= OnChange;
      }
      player = null;
      if (!string.IsNullOrEmpty(playerName))
      {

        player = PlayerManager.Instance.LocalPlayer ?? PlayerManager.Instance.PlayersByName[playerName];
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
        if (PlayerLevel != null)
          PlayerLevel.text = "";
        if (HealthBar != null)
          HealthBar.fillAmount = 0;
        if (ManaBar != null)
          ManaBar.fillAmount = 0;
        if (HealthText != null)
          HealthText.text = "?? / ??";
        if (ManaText != null)
          ManaText.text = "?? / ??";
        return;
      }
      if (PlayerLevel != null)
        PlayerLevel.text = "LV " + player.Stats.Level.Value + " " + player.Stats.ClassType.ClassName;
      // convert a value to a float to retain the decimal value
      if (HealthBar != null)
        HealthBar.fillAmount = player.Stats.Energies.Value.Health / (player.Stats.Energies.Value.MaxHealth * 1f);
      if (ManaBar != null)
        ManaBar.fillAmount = player.Stats.Energies.Value.Mana / (player.Stats.Energies.Value.MaxMana * 1f);
      if (HealthText != null)
        HealthText.text = player.Stats.Energies.Value.Health + " / " + player.Stats.Energies.Value.MaxHealth;
      if (ManaText != null)
        ManaText.text = player.Stats.Energies.Value.Mana + " / " + player.Stats.Energies.Value.MaxMana;
    }

    // public void OpenContextMenu()
    // {

    //   PartyUI.Instance.PlayerContextMenu.ContextualGameObject = gameObject;
    //   PartyUI.Instance.OpenPlayerContextMenu(transform.position, playerName);
    // }
  }
}