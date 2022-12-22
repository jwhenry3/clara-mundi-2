using System;
using System.Collections.Generic;
using Backend.App;
using TMPro;
using Unisave.Facades;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class CharactersUI : MonoBehaviour
    {
        public TextMeshProUGUI StatusMessage;
        public Transform CharactersContainer;
        public GameObject CharacterActions;
        public List<CharacterEntity> Characters;

        public CharacterUI CharacterPrefab;

        public static CharactersUI Instance;

        private void Awake()
        {
            Instance = this;
            CharactersContainer.gameObject.SetActive(false);
            CharacterActions.gameObject.SetActive(false);
        }

        public void Select(CharacterEntity character)
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
            string characterName = SessionManager.Instance.PlayerCharacter.Name;
            StatusMessage.enabled = true;
            StatusMessage.text = "Deleting " + characterName+ "...";
            var result = await OnFacet<CharacterFacet>.CallAsync<bool>(
                nameof(CharacterFacet.DeleteCharacter),
                SessionManager.Instance.PlayerCharacter.Name
            );
            if (result)
                OnEnable();
            else
                StatusMessage.text = "Could not delete the character";
        }

        private async void OnEnable()
        {
            Select(null);
            StatusMessage.enabled = true;
            StatusMessage.text = "Loading Characters...";
            CharactersContainer.gameObject.SetActive(false);
            CharacterActions.gameObject.SetActive(false);
            foreach (Transform child in CharactersContainer)
                Destroy(child.gameObject);
            // get characters from server
            Characters = await OnFacet<CharacterFacet>.CallAsync<List<CharacterEntity>>(
                nameof(CharacterFacet.GetCharacters)
            );
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
        }

        private void OnDisable()
        {
            SessionManager.Instance.PlayerCharacter = null;
        }
        private void OnDestroy()
        {
            SessionManager.Instance.PlayerCharacter = null;
        }

        private void AddCharacter(CharacterEntity character)
        {
            Instantiate(CharacterPrefab, CharactersContainer, false).SetCharacter(character);
        }
    }
}