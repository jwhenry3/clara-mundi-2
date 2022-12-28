using System;
using System.Threading.Tasks;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class LobbyUI : MonoBehaviour
    {
        public GameObject Background;
        public Transform LoginRegisterPanel;
        public Transform CharacterSelectionPanel;
        public Transform CreateCharacterPanel;
        public LoginUI LoginUI;
        public RegisterUI RegisterUI;

        public static LobbyUI Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Background.SetActive(true);
            CloseAll();
            CheckAccount();
        }

        public void CheckAccount()
        {
            if (SessionManager.Instance.PlayerAccount != null)
                ToCharacterSelection();
            else
                ToLogin();
        }

        public async void Logout()
        {
            ToLogin();
            await OnFacet<AccountFacet>.CallAsync(
                nameof(AccountFacet.Logout)
            );
        }

        public void ToLogin()
        {
            SessionManager.Instance.Clear();
            Client.Instance.Disconnect();
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(true);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }

        public void ToRegister()
        {
            SessionManager.Instance.Clear();
            Client.Instance.Disconnect();
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(true);
            LoginUI.gameObject.SetActive(false);
            RegisterUI.gameObject.SetActive(true);
        }
        public void ToCharacterSelection()
        {
            SessionManager.Instance.PlayerCharacter = null;
            Client.Instance.Disconnect();
            SessionManager.Instance.PlayerCharacter = null;
            CharacterSelectionPanel.gameObject.SetActive(true);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
        public void ToCreateCharacter()
        {
            SessionManager.Instance.PlayerCharacter = null;
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(true);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
        public void CloseAll()
        {
            SessionManager.Instance.Clear();
            Client.Instance.Disconnect();
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
    }
}