using TMPro;
using UnityEngine;

namespace ClaraMundi
{
  public class LoginUIBackup : MonoBehaviour
  {
    public InputFieldWithHybridNav EmailField;
    public InputFieldWithHybridNav PasswordField;

    public TextMeshProUGUI StatusMessage;

    public void OnEnable()
    {
      EmailField.text = "";
      PasswordField.text = "";
      StatusMessage.text = "";
      StatusMessage.enabled = false;
      EmailField.SubmitAction += Submit;
      PasswordField.SubmitAction += Submit;
    }

    public void OnDisable()
    {
      EmailField.SubmitAction -= Submit;
      PasswordField.SubmitAction -= Submit;
    }

    public async void Submit()
    {
      StatusMessage.enabled = true;
      StatusMessage.text = "Logging in...";
      Debug.Log(EmailField.text + " - " + PasswordField.text);
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