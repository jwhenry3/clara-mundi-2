using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace ClaraMundi
{
  public class OnHook : MonoBehaviour
  {

    public UnityEvent Disabled = new();
    public UnityEvent Enabled = new();
    public void OnDisable()
    {
      Disabled?.Invoke();
    }
    public void OnEnable()
    {
      Enabled?.Invoke();
    }

  }
}