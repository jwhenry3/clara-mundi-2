using System;
using Unisave.Facades;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ClaraMundi
{
    public class LoginForm : MonoBehaviour
    {
        public TMP_InputField emailField;
        public TMP_InputField passwordField;
        public TextMeshProUGUI statusText;

        public string sceneAfterLogin;

        void Start()
        {
            if (emailField == null)
                throw new ArgumentException(
                    $"Link the '{nameof(emailField)}' in the inspector."
                );

            if (passwordField == null)
                throw new ArgumentException(
                    $"Link the '{nameof(passwordField)}' in the inspector."
                );

            if (statusText == null)
                throw new ArgumentException(
                    $"Link the '{nameof(statusText)}' in the inspector."
                );

            statusText.enabled = false;
        }

        void OnEnable()
        {
            emailField.text = "";
            passwordField.text = "";
            statusText.text = "";
        }
        public async void OnLoginClicked()
        {
            statusText.enabled = true;
            statusText.text = "Logging in...";

            var response = await OnFacet<EmailLoginFacet>.CallAsync<string>(
                nameof(EmailLoginFacet.Login),
                emailField.text,
                passwordField.text
            );
            AuthHolder.Token = response;

            statusText.text = response != "" 
                ? "Success!" 
                : "Given credentials are not valid";
        }
    }

}