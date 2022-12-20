using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class CreateCharacterUI : MonoBehaviour
    {
        
        public TMP_InputField NameField;
        public TMP_Dropdown RaceDropdown;
        public TMP_Dropdown GenderDropdown;
        
        
        public void OnEnable()
        {
            RaceDropdown.value = 0;
            GenderDropdown.value = 0;
            NameField.text = "";
        }
    }
}