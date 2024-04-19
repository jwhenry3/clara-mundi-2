using UnityEngine;

namespace ClaraMundi
{
  public class GameObjectUtils
  {

    public static void SetActive(MonoBehaviour monoBehaviour, bool value)
    {
      monoBehaviour.gameObject.SetActive(value);
    }

  }
}