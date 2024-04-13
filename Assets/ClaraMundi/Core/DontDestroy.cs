using UnityEngine;

namespace ClaraMundi
{
  public class DontDestroy : MonoBehaviour
  {

    void Start()
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}