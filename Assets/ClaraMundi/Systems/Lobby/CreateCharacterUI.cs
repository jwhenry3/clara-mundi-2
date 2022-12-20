using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class CreateCharacterUI : MonoBehaviour
    {
        
        public TMP_InputField NameField;
        
        
        public void OnEnable()
        {
            NameField.text = "";
        }
    }
}