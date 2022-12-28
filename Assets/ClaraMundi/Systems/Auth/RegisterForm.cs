using System;
using UnityEngine;
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

            var response = await Authentication.Register(emailField.text, passwordField.text);
            AuthHolder.Token = response.account?.token;

            statusText.text = response.status
                ? "Success!" 
                : "Given credentials are not valid";

            switch (response.reason)
            {
                case "":
                    // SceneManager.LoadScene(sceneAfterRegistration);
                    statusText.text = "Success!";
                    break;

                case "conflict":
                    statusText.text = "This email has already been registered";
                    break;
                default:
                    statusText.text = "Unknown response: " + response.reason;
                    break;
            }
        }
    }

}