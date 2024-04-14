using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
  public class CharacterUI : MonoBehaviour
  {
    public Character Character;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Area;

    public GameObject Outline;

    public void OnSelect()
    {
      CharactersUI.Instance.Select(Character);
    }

    public void OnToServerSelection()
    {
      CharactersUI.Instance.Select(Character);
      CharactersUI.Instance.OnServerSelection();
    }
    public void Delete()
    {
      CharactersUI.Instance.Select(Character);
      CharactersUI.Instance.OnDelete();
    }

    public void SetCharacter(Character character)
    {
      Character = character;
      Level.text = "LV " + character.level;
      Name.text = character.name;
      Area.text = character.area;
    }

    public void OnChange(Character character)
    {
      Outline.SetActive(character?.name == Character.name);
    }

    public void OnEnable()
    {
      CharactersUI.Instance.OnChange += OnChange;
      if (SessionManager.Instance.PlayerCharacter == Character &&
          EventSystem.current.currentSelectedGameObject != gameObject)
        EventSystem.current.SetSelectedGameObject(gameObject);
      Outline.SetActive(SessionManager.Instance.PlayerCharacter?.name == Character.name);
    }

    public void OnDisable()
    {
      CharactersUI.Instance.OnChange -= OnChange;
    }
  }
}