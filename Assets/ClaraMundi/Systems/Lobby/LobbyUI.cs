using System;
using System.Threading.Tasks;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class LobbyUI : MonoBehaviour
    {
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
            CheckAccount();
        }

        public async void CheckAccount()
        {
            CloseAll();
            if (await GetAccount())
                ToCharacterSelection();
            else
                ToLogin();
        }

        public async Task<bool> GetAccount()
        {
            GameManager.Instance.PlayerAccount = await OnFacet<AccountFacet>.CallAsync<AccountEntity>(
                nameof(AccountFacet.GetAccount)
            );
            return GameManager.Instance.PlayerAccount != null;
        }

        public async void Logout()
        {
            GameManager.Instance.PlayerAccount = null;
            ToLogin();
            await OnFacet<AccountFacet>.CallAsync(
                nameof(AccountFacet.Logout)
            );
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
        }
        public void ToCreateCharacter()
        {
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(true);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
        public void CloseAll()
        {
            CharacterSelectionPanel.gameObject.SetActive(false);
            CreateCharacterPanel.gameObject.SetActive(false);
            LoginRegisterPanel.gameObject.SetActive(false);
            LoginUI.gameObject.SetActive(true);
            RegisterUI.gameObject.SetActive(false);
        }
    }
}