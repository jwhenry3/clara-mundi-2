using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class CharacterUI : MonoBehaviour
    {
        public CharacterEntity CharacterEntity;
        public TextMeshProUGUI Level;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Area;

        public void OnSelect()
        {
            CharactersUI.Instance.Select(CharacterEntity);
        }

        public void SetCharacter(CharacterEntity entity)
        {
            CharacterEntity = entity;
            Level.text = "LV " + entity.Level;
            Name.text = entity.Name;
            Area.text = entity.Area;
        }
    }
}