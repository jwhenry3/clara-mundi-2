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

        public CharacterEntity SelectedCharacter;

        private void Awake()
        {
            Instance = this;
            CharactersContainer.gameObject.SetActive(false);
            CharacterActions.gameObject.SetActive(false);
        }

        public void Select(CharacterEntity character)
        {
            if (SelectedCharacter == character) return;
            CharacterActions.SetActive(character != null);
            SelectedCharacter = character;
        }

        public void OnEnterGame()
        {
            if (SelectedCharacter == null) return;
            Debug.Log("Enter Game!");
        }

        public void OnDelete()
        {
            if (SelectedCharacter == null) return;
            Debug.Log("Delete Character!");
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
            {
                LobbyUI.Instance.ToCreateCharacter();
                return;
            }


            foreach (var character in Characters)
                AddCharacter(character);
            CharactersContainer.gameObject.SetActive(true);
            StatusMessage.enabled = false;
        }

        private void AddCharacter(CharacterEntity character)
        {
            var instance = Instantiate(CharacterPrefab, CharactersContainer, false);
            instance.SetCharacter(character);
        }

        private float updateTick;
        private void LateUpdate()
        {
            updateTick += Time.deltaTime;
            if (!(updateTick > 1)) return;
            updateTick = 0;
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject == null)
                Select(null);
            else if (selectedObject.GetComponent<CharacterUI>() == null)
                Select(null);
        }
    }
}