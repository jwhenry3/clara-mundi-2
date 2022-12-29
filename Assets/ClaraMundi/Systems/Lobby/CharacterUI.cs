using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class CharacterUI : MonoBehaviour
    {
        public Character Character;
        public TextMeshProUGUI Level;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Area;

        public void OnSelect()
        {
            CharactersUI.Instance.Select(Character);
        }

        public void SetCharacter(Character character)
        {
            Character = character;
            Level.text = "LV " + character.level;
            Name.text = character.name;
            Area.text = character.area;
        }

        public void Update()
        {
            if (SessionManager.Instance.PlayerCharacter == Character &&
                EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}