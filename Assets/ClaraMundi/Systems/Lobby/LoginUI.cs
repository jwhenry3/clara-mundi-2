using System;
using TMPro;
using Unisave.Facades;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClaraMundi
{
    public class LoginUI : MonoBehaviour
    {
        public TMP_InputField EmailField;
        public TMP_InputField PasswordField;

        public TextMeshProUGUI StatusMessage;

        public void OnEnable()
        {
            EmailField.text = "";
            PasswordField.text = "";
            StatusMessage.text = "";
            StatusMessage.enabled = false;
        }

        public async void Submit()
        {
            StatusMessage.enabled = true;
            StatusMessage.text = "Logging in...";

            string response = await OnFacet<EmailLoginFacet>.CallAsync<string>(
                nameof(EmailLoginFacet.Login),
                EmailField.text,
                PasswordField.text
            );

            if (!string.IsNullOrEmpty(response))
            {
                LobbyUI.Instance.CheckAccount();
                StatusMessage.text = "Login succeeded";
            }
            else
            {
                StatusMessage.text = "Invalid Email or Password";
            }
        }
    }
}