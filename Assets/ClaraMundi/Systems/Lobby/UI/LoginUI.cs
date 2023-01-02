using TMPro;
using UnityEngine;

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

            var response = await LobbyApi.Login(
                EmailField.text,
                PasswordField.text
            );

            if (response.status)
            {
                SessionManager.Instance.PlayerAccount = response.account;

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