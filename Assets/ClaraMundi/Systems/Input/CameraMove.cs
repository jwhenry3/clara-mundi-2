using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class CameraMove : MonoBehaviour
  {

    public ShotCamera PlayerCamera;
    public MainCamera MainCamera;
    private InputAction InputAction;
    private ShotTypeThirdPerson shotType;
    private ShotSystemThirdPerson system;


    void OnEnable()
    {
      if (InputAction != null)
      {
        InputAction.performed += OnPerform;
      }
      shotType = (ShotTypeThirdPerson)PlayerCamera.ShotType;
      system = (ShotSystemThirdPerson)shotType.GetSystem(ShotSystemThirdPerson.ID);
    }
    void OnDisable()
    {
      InputAction.performed -= OnPerform;
    }

    void Update()
    {
      if (InputAction == null && InputManager.Instance.World != null)
      {
        InputAction = InputManager.Instance.World.FindAction("Look");
        InputAction.performed += OnPerform;
      }
      if (shotType != null && MainCamera.Transition.CurrentShotCamera == PlayerCamera)
      {
        if (!shotType.IsActive)
        {
          shotType.OnEnable(MainCamera);
          // Debug.Log("Enabled Shot Type");
        }
      }
      else
      {
        if (shotType.IsActive)
        {
          shotType.OnDisable(MainCamera);
          // Debug.Log("Disabled Shot Type");
        }
      }
    }

    void OnPerform(InputAction.CallbackContext context)
    {
      var value = context.ReadValue<Vector2>().normalized;
      // alter camera orthographic size if player shot is active
      if (MainCamera.Transition.CurrentShotCamera == PlayerCamera)
      {
      }
    }
  }
}