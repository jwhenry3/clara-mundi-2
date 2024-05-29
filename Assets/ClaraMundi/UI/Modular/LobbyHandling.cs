using UnityEngine;
namespace ClaraMundi
{
  public class LobbyHandling : MonoBehaviour
  {
    public static LobbyHandling Instance;
    public WindowUI Login;
    public WindowUI Register;
    public WindowUI Characters;
    public WindowUI NewCharacter;

    void OnEnable()
    {
      SessionManager.Instance.PlayerAccount = null;
      Instance = this;
    }
    void Update()
    {
      if (SessionManager.Instance.PlayerAccount == null)
      {
        if (!Login.gameObject.activeInHierarchy && !Register.gameObject.activeInHierarchy)
        {
          Login.moveSibling.ToFront();
        }
      }
      else
      {
        if (!Characters.gameObject.activeInHierarchy && !NewCharacter.gameObject.activeInHierarchy)
        {
          Characters.moveSibling.ToFront();
        }
      }
    }
  }
}