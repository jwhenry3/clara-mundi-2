﻿using System.Collections.Generic;
using Backend.App;
using TMPro;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class CreateCharacterUI : MonoBehaviour
    {
        public TMP_InputField NameField;
        public TMP_Dropdown RaceDropdown;
        public TMP_Dropdown GenderDropdown;
        public TextMeshProUGUI StatusMessage;

        private string[] RaceOptions = new[] { "Human" };
        private string[] GenderOptions = new[] { "Male", "Female" };

        public void OnEnable()
        {
            RaceDropdown.value = 0;
            GenderDropdown.value = 0;
            NameField.text = "";
            StatusMessage.text = "";
            StatusMessage.enabled = false;
        }

        public async void OnCreate()
        {
            if (RaceDropdown.value == -1 || RaceDropdown.value > 0) return;
            if (GenderDropdown.value == -1 || GenderDropdown.value > 1) return;
            StatusMessage.enabled = true;
            StatusMessage.text = "Creating Character...";
            if (string.IsNullOrEmpty(NameField.text) || string.IsNullOrWhiteSpace(NameField.text)) return;
            var result = await LobbyApi.CreateCharacter(NameField.text, GenderOptions[GenderDropdown.value],
                RaceOptions[RaceDropdown.value]);
            Debug.Log(result.reason);
            if (result.status)
            {
                SessionManager.Instance.PlayerCharacter = result.character;
                Client.Instance.Connect();
            }
            else
            {
                StatusMessage.text = "Could not create the character, try a different name.";
            }
        }
    }
}