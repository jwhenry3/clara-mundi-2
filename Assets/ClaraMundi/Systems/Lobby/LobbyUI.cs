using System;
using UnityEngine;

namespace ClaraMundi
{
    public class LobbyUI : MonoBehaviour
    {
        public Transform LoginRegisterPanel;
        public Transform CharacterSelectionPanel;
        public Transform CreateCharacterPanel;
        public CharactersUI CharactersUI;
        public LoginUI LoginUI;
        public RegisterUI RegisterUI;
        public CreateCharacterUI CreateCharacterUI;
        public bool StartAtCharacterList;

        public static LobbyUI Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            if (StartAtCharacterList)
                ToCharacterSelection();
            else
                ToLogin();
        }

        public void ToLogin()
        {
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(true);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }

        public void ToRegister()
        {
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(true);
            LoginUI.gameObject.SetActive(false);
            RegisterUI.gameObject.SetActive(true);
        }
        public void ToCharacterSelection()
        {
            CharacterSelectionPanel.gameObject.SetActive(true);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
            StartAtCharacterList = false;
        }
        public void ToCreateCharacter()
        {
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(true);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
    }
}