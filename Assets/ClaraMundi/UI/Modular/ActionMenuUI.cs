using UnityEngine;
namespace ClaraMundi
{
  public class ActionMenuUI : MonoBehaviour
  {
    public static ActionMenuUI Instance;
    public Entity targetEntity;
    public WindowUI window;

    void OnEnable()
    {
      Instance = this;
      window = window ?? GetComponent<WindowUI>();
    }

    public void SetVisible(string name, bool value)
    {
      GameObject button = GameObject.Find("ActionMenu/" + name);
      button.SetActive(value);
    }

    public void OnFight()
    {

    }

    public void OnSkills()
    {

    }
    public void OnDisengage() { }

    public void OnChat() { }
    public void OnTrade() { }
    public void OnInvite() { }
    public void OnCheck() { }
  }
}