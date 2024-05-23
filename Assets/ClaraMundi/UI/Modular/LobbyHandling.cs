using UnityEngine;
namespace ClaraMundi
{
  public class LobbyHandling : MonoBehaviour
  {
    public static LobbyHandling Instance;
    public WindowUI Login;
    public WindowUI Register;
    public WindowUI Characters;

    void OnEnable()
    {
      SessionManager.Instance.PlayerAccount = null;
      Instance = this;
    }
    void Update()
    {
      if (SessionManager.Instance.PlayerAccount == null)
      {
        if (!Login.moveSibling.IsInFront() && !Register.moveSibling.IsInFront())
        {
          Login.moveSibling.ToFront();
        }
      }
      else
      {
        if (!Characters.moveSibling.IsInFront())
          Characters.moveSibling.ToFront();
      }
    }
  }
}