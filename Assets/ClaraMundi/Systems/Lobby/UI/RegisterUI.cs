using TMPro;
using Unisave.Facades;
using UnityEngine;

namespace ClaraMundi
{
    public class RegisterUI : MonoBehaviour
    {
        
        public TMP_InputField EmailField;
        public TMP_InputField PasswordField;
        public TMP_InputField ConfirmPasswordField;

        public TextMeshProUGUI StatusMessage;

        public void OnEnable()
        {
            EmailField.text = "";
            PasswordField.text = "";
            ConfirmPasswordField.text = "";
            StatusMessage.text = "";
            StatusMessage.enabled = false;
        }
        
        public async void Submit()
        {
            
            StatusMessage.enabled = true;
            StatusMessage.text = "Registering...";

            if (PasswordField.text != ConfirmPasswordField.text)
            {
                StatusMessage.text = "Password confirmation does not match";
                return;
            }
        
            var response = await LobbyApi.Register(
                EmailField.text,
                PasswordField.text
            );

            switch (response.reason)
            {
                case "":
                    StatusMessage.text = "Registration succeeded";
                    SessionManager.Instance.PlayerAccount = response.account;
                    LobbyUI.Instance.CheckAccount();
                    break;
            
                case "conflict":
                    StatusMessage.text = "This email has already been registered";
                    break;
            
                default:
                    StatusMessage.text = "Unknown response: " + response.reason;
                    break;
            }
        }
    }
}