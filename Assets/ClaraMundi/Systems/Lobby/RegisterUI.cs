using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class RegisterUI : MonoBehaviour
    {
        
        public TMP_InputField EmailField;
        public TMP_InputField PasswordField;
        public TMP_InputField ConfirmPasswordField;

        public TextMeshProUGUI ErrorMessage;

        public void OnEnable()
        {
            EmailField.text = "";
            PasswordField.text = "";
            ConfirmPasswordField.text = "";
            ErrorMessage.text = "";
        }
        
        public void Submit()
        {
            
        }
    }
}