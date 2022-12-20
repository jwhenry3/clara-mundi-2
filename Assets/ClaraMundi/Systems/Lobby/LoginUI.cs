using System;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class LoginUI : MonoBehaviour
    {
        public TMP_InputField EmailField;
        public TMP_InputField PasswordField;

        public TextMeshProUGUI ErrorMessage;

        public void OnEnable()
        {
            EmailField.text = "";
            PasswordField.text = "";
            ErrorMessage.text = "";
        }

        public void Submit()
        {
            
        }
    }
}