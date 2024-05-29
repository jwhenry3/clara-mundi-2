using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ClaraMundi
{
  public class CreateCharacterFormUI : FormUI
  {

    public TMP_Dropdown RaceDropdown;
    public TMP_Dropdown GenderDropdown;
    public TMP_InputField NameInput;
    public TextMeshProUGUI StatusMessage;
    public ButtonUI Create;
    public UnityEvent OnComplete = new();
    public void OnEnable()
    {
      RaceDropdown.value = 0;
      GenderDropdown.value = 0;
      NameInput.text = "";
      StatusMessage.text = "";
      StatusMessage.enabled = false;
    }
    public async override void Submit()
    {
      StatusMessage.text = "Creating Character...";
      if (string.IsNullOrEmpty(NameInput.text) || string.IsNullOrWhiteSpace(NameInput.text)) return;
      var result = await LobbyApi.CreateCharacter(
          NameInput.text,
          GenderDropdown.options[GenderDropdown.value].text,
          RaceDropdown.options[RaceDropdown.value].text,
          "Adventurer"
      );

      if (result.status)
      {
        SessionManager.Instance.PlayerCharacter = result.character;
        OnComplete?.Invoke();
      }
      else
      {
        StatusMessage.text = "Could not create the character, try a different name.";
      }

    }

    void Update()
    {
      Create.button.interactable = NameInput.text.Length > 2;
    }
  }
}