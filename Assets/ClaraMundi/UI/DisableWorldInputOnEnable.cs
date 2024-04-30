using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class DisableWorldInputOnEnable : MonoBehaviour
  {

    public InputActionAsset InputActionAsset;

    void OnEnable()
    {
      InputActionAsset.FindActionMap("Player").Disable();
    }

    void OnDisable()
    {
      InputActionAsset.FindActionMap("Player").Enable();
    }
  }
}