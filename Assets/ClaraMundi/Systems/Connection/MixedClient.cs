using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClaraMundi {
  public class MixedClient : MonoBehaviour {


      public void StartServer() {
        SceneManager.LoadScene("GameServer");
      }
      public void StartClient() {
        SceneManager.LoadScene("GameClient");
      }
  }
}