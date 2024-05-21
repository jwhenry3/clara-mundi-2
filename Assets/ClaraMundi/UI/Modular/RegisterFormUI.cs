using UnityEngine;
using TMPro;
namespace ClaraMundi
{
  public class RegisterFormUI : FormUI
  {
    public InputUI Email;
    public InputUI Password;
    public InputUI ConfirmPassword;
    public TextMeshProUGUI StatusMessage;


    void OnDisable()
    {
      StatusMessage.text = " ";
    }
    public async override void Submit()
    {
      StatusMessage.text = "Registering...";

      if (Password.inputField.text != ConfirmPassword.inputField.text)
      {
        StatusMessage.text = "Password confirmation does not match";
        return;
      }

      var response = await LobbyApi.Register(
          Email.inputField.text,
          Password.inputField.text
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