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
        
            var response = await OnFacet<EmailRegisterFacet>
                .CallAsync<RegistrationResponse>(
                    nameof(EmailRegisterFacet.Register),
                    EmailField.text,
                    PasswordField.text
                );

            switch (response.status)
            {
                case EmailRegisterResponse.Ok:
                    StatusMessage.text = "Registration succeeded";
                    LobbyUI.Instance.CheckAccount();
                    break;
            
                case EmailRegisterResponse.EmailTaken:
                    StatusMessage.text = "This email has already been registered";
                    break;
            
                case EmailRegisterResponse.InvalidEmail:
                    StatusMessage.text = "This is not a valid email address";
                    break;
            
                case EmailRegisterResponse.WeakPassword:
                    StatusMessage.text = "Password needs to be at least 8 " +
                                         "characters long";
                    break;
            
                default:
                    StatusMessage.text = "Unknown response: " + response;
                    break;
            }
        }
    }
}