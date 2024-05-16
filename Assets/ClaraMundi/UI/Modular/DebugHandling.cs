using UnityEngine;
namespace ClaraMundi
{
  public class DebugHandling : MonoBehaviour
  {
    public bool IsDebug;
    public GameObject MainUI;

    void OnEnable()
    {
      foreach (Transform child in transform)
      {
        child.gameObject.SetActive(IsDebug);
      }
      MainUI.SetActive(true);
    }
  }
}