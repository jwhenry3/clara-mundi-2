using System;
using Unisave.Facades;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ClaraMundi
{
    public class RegisterForm : MonoBehaviour
    {
        public TMP_InputField emailField;
        public TMP_InputField passwordField;
        public TMP_InputField confirmPasswordField;
        public TextMeshProUGUI statusText;

        public string sceneAfterRegistration;

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

            if (confirmPasswordField == null)
                throw new ArgumentException(
                    $"Link the '{nameof(confirmPasswordField)}' in the inspector."
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
            confirmPasswordField.text = "";
            statusText.text = "";
        }

        public async void OnRegisterClicked()
        {
            statusText.enabled = true;
            statusText.text = "Registering...";

            if (passwordField.text != confirmPasswordField.text)
            {
                statusText.text = "Password confirmation does not match";
                return;
            }

            var response = await OnFacet<EmailRegisterFacet>
                .CallAsync<RegistrationResponse>(
                    nameof(EmailRegisterFacet.Register),
                    emailField.text,
                    passwordField.text
                );

            switch (response.status)
            {
                case EmailRegisterResponse.Ok:
                    // SceneManager.LoadScene(sceneAfterRegistration);
                    AuthHolder.Token = response.token;
                    statusText.text = "Success!";
                    break;

                case EmailRegisterResponse.EmailTaken:
                    statusText.text = "This email has already been registered";
                    break;

                case EmailRegisterResponse.InvalidEmail:
                    statusText.text = "This is not a valid email address";
                    break;

                case EmailRegisterResponse.WeakPassword:
                    statusText.text = "Password needs to be at least 8 " +
                                      "characters long";
                    break;

                default:
                    statusText.text = "Unknown response: " + response;
                    break;
            }
        }
    }

}