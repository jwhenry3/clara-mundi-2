using System.Collections.Generic;
using Backend.App;
using TMPro;
using Unisave.Facades;
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

        private void Awake()
        {
            Instance = this;
            CharactersContainer.gameObject.SetActive(false);
            CharacterActions.gameObject.SetActive(false);
        }

        public void Select(Character character)
        {
            if (SessionManager.Instance.PlayerCharacter == character) return;
            CharacterActions.SetActive(character != null);
            SessionManager.Instance.PlayerCharacter = character;
        }

        public void OnEnterGame()
        {
            if (SessionManager.Instance.PlayerAccount == null) return;
            if (SessionManager.Instance.PlayerCharacter == null) return;
            Client.Instance.Connect();
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

        private async void OnEnable()
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
            Characters = response.characters;

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
            loading = false;
        }

        private void OnDisable()
        {
            loading = false;
            SessionManager.Instance.PlayerCharacter = null;
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