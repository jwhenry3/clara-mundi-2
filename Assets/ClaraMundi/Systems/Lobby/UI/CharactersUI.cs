using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
  public class CharactersUI : MonoBehaviour
  {
    public TextMeshProUGUI StatusMessage;
    public Transform CharactersContainer;
    public GameObject CharacterActions;
    public List<Character> Characters;

    public CharacterUI CharacterPrefab;

    public static CharactersUI Instance;

    public Action<Character> OnChange;

    public Form Form;

    private void Start()
    {
      Instance = this;
      CharactersContainer.gameObject.SetActive(false);
      CharacterActions.gameObject.SetActive(false);
    }

    public void Select(Character character)
    {
      OnChange?.Invoke(character);
      if (SessionManager.Instance.PlayerCharacter?.name == character?.name) return;
      CharacterActions.SetActive(character != null);
      SessionManager.Instance.PlayerCharacter = character;
      GetComponent<Form>().InitializeElements();

    }

    public void OnServerSelection()
    {
      if (SessionManager.Instance.PlayerAccount == null) return;
      if (SessionManager.Instance.PlayerCharacter == null) return;
      LobbyUI.Instance.ToServerList();
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

    private bool loading;

    private void OnEnable()
    {
      LoadCharacters();
    }

    public async void LoadCharacters()
    {
      if (loading) return;
      loading = true;
      Select(null);
      StatusMessage.enabled = true;
      StatusMessage.text = "Loading Characters...";
      CharacterActions.gameObject.SetActive(false);
      foreach (Transform child in CharactersContainer)
        Destroy(child.gameObject);

      var response = await LobbyApi.GetCharacters();
      Characters = new();
      if (response.characters != null)
      {
        Characters = response.characters.Select(c => Character.FromData(c)).ToList();
      }

      if (!isActiveAndEnabled) return;
      if (Characters.Count == 0)
        StatusMessage.text = "You have no characters. Please create one.";
      else
      {
        foreach (var character in Characters)
          AddCharacter(character);
        CharactersContainer.gameObject.SetActive(true);
        StatusMessage.enabled = false;
      }

      CharactersContainer.gameObject.SetActive(Characters.Count > 0);
      Form?.InitializeElements();
      loading = false;
    }

    private void OnDisable()
    {
      loading = false;
      foreach (Transform child in CharactersContainer)
        Destroy(child.gameObject);
    }

    private void OnDestroy()
    {
      loading = false;
      SessionManager.Instance.PlayerCharacter = null;
    }

    private void AddCharacter(Character character)
    {
      Instantiate(CharacterPrefab, CharactersContainer, false).SetCharacter(character);
    }
  }
}