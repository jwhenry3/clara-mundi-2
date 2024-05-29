using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ClaraMundi
{
  public class CharacterSelectionUI : MonoBehaviour
  {
    public TMP_Dropdown CharacterDropdown;
    public TMP_Dropdown ServerDropdown;
    public TextMeshProUGUI StatusMessage;
    public ButtonUI Delete;
    public ButtonUI EnterGame;
    bool loading;


    private List<Character> Characters = new();
    private List<KeyValuePair<string, ServerEntry>> Servers = new();
    void OnEnable()
    {
      StatusMessage.text = " ";
      Load();
    }

    void OnDisable()
    {
      StatusMessage.text = " ";
      ServerDropdown.options = new();
      ServerDropdown.value = 0;
      CharacterDropdown.options = new();
      CharacterDropdown.value = 0;
    }
    public async void Load()
    {
      if (loading) return;
      if (SessionManager.Instance.PlayerAccount == null) return;
      CharacterDropdown.options = new() {
        new () {
          text = "Loading characters..."
        }
      };
      ServerDropdown.options = new() {
        new () {
          text = "Loading servers..."
        }
      };
      StatusMessage.text = " ";
      await LoadServers();
      await LoadCharacters();
      loading = false;
    }
    private async Task<int> LoadServers()
    {
      ServerDropdown.options = new();
      await MasterServerApi.Instance.GetServerList();
      Servers = MasterServerApi.Instance.serversByLabel.ToList();
      foreach (var kvp in Servers)
      {
        ServerDropdown.options.Add(new()
        {
          text = kvp.Value.label + " - " + (kvp.Value.status ? "Up" : "Down") + " - " + $"{kvp.Value.currentPlayers}/{kvp.Value.playerCapacity}"
        });
      }
      if (ServerDropdown.options.Count == 0)
      {
        ServerDropdown.options.Add(new()
        {
          text = "No Servers"
        });
      }
      return Servers.Count;
    }
    private Character FromData(CharacterData data)
    {
      return new()
      {
        name = data.name,
        gender = data.gender,
        race = data.race,
        area = data.area,
        level = data.level ?? 1,
        exp = data.exp ?? 0,
        classId = data.classid ?? ""
      };
    }
    private async Task<int> LoadCharacters()
    {
      CharacterDropdown.options = new();
      Characters = new();

      var response = await LobbyApi.GetCharacters();
      if (response.characters != null)
      {
        Characters = response.characters.Select(c => FromData(c)).ToList();
        foreach (Character character in Characters)
        {
          CharacterDropdown.options.Add(new()
          {
            text = "LV " + character.level + " " + character.name
          });
        }
      }

      if (Characters.Count == 0)
      {
        CharacterDropdown.options.Add(new()
        {
          text = "No Characters"
        });
      }
      return Characters.Count;
    }
    public async void OnDelete()
    {
      if (SessionManager.Instance.PlayerCharacter == null) return;
      string characterName = SessionManager.Instance.PlayerCharacter.name;
      StatusMessage.enabled = true;
      StatusMessage.text = "Deleting " + characterName + "...";
      var result = await LobbyApi.DeleteCharacter(characterName);
      if (result.status)
        OnEnable();
      else
        StatusMessage.text = "Could not delete the character";
    }
    public void OnEnterGame()
    {
      if (Characters.Count == 0)
      {
        StatusMessage.text = "Select or Create a Character";
        return;
      }
      if (Servers.Count == 0)
      {
        StatusMessage.text = "No Servers to Connect to";
        return;
      }
      if (CharacterDropdown.value + 1 > Characters.Count) return;
      if (ServerDropdown.value + 1 > Characters.Count) return;

      SessionManager.Instance.PlayerCharacter = Characters[CharacterDropdown.value];
      Client.Instance.SelectedServer = Servers[ServerDropdown.value].Value;
      Client.Instance.Connect();
      Debug.Log("Can Enter Game!");
    }

    void Update()
    {
      var playerSelected = SessionManager.Instance.PlayerCharacter != null;
      Delete.button.interactable = playerSelected;
      EnterGame.button.interactable = playerSelected;
      if (CharacterDropdown.options.Count > CharacterDropdown.value)
        CharacterDropdown.captionText.text = CharacterDropdown.options[CharacterDropdown.value].text;
      else
        CharacterDropdown.captionText.text = "No Characters Yet";
      if (ServerDropdown.options.Count > ServerDropdown.value)
        ServerDropdown.captionText.text = ServerDropdown.options[ServerDropdown.value].text;
      else
        ServerDropdown.captionText.text = "No Servers Found";
    }
  }
}