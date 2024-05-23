using UnityEngine;
using TMPro;

namespace ClaraMundi
{
  public class LoginFormUI : FormUI
  {
    public InputUI Email;
    public InputUI Password;
    public TextMeshProUGUI StatusMessage;

    void OnDisable()
    {
      StatusMessage.text = " ";
    }
    public async override void Submit()
    {
      StatusMessage.text = "Logging in...";

      var response = await LobbyApi.Login(
          Email.inputField.text,
          Password.inputField.text
      );

      if (response.status)
      {
        SessionManager.Instance.PlayerAccount = response.account;

        StatusMessage.text = "Login succeeded";
      }
      else
      {
        StatusMessage.text = "Invalid Email or Password";
      }
    }
  }
}