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
    public ShotCamera PlayerTargetCamera;
    public MainCamera MainCamera;
    private InputAction LookAction;
    private InputAction ZoomAction;
    private ShotTypeThirdPerson shotType;
    private ShotTypeLockOn lockOnShotType;
    private ShotSystemThirdPerson system;

    private Vector3 lockOnLimit = new Vector3(3, 3, 3);
    private Vector3 offset = new Vector3(0, 1, 0);

    public Player player => PlayerManager.Instance != null ? PlayerManager.Instance.LocalPlayer : null;


    void OnEnable()
    {
      shotType = (ShotTypeThirdPerson)PlayerCamera.ShotType;
      system = (ShotSystemThirdPerson)shotType.GetSystem(ShotSystemThirdPerson.ID);
    }

    void Update()
    {
      if (LookAction == null && InputManager.Instance.World != null)
      {
        LookAction = InputManager.Instance.World.FindAction("Look");
        ZoomAction = InputManager.Instance.World.FindAction("Zoom");
      }
      if (player != null)
      {
        if (player.Targeting.cameraLockSubTarget && !string.IsNullOrEmpty(player.Targeting.SubTargetId))
        {
          lockOnShotType = player.Targeting.SubTarget.lockOnShotType;
          if (lockOnShotType != null && MainCamera.Transition.CurrentShotCamera == PlayerCamera)
          {
            if (shotType.IsActive)
              shotType.OnDisable(MainCamera);
            if (!lockOnShotType.IsActive)
              lockOnShotType.OnEnable(MainCamera);
          }
          if (CameraManager.Instance.MainCamera.Transition.CurrentShotCamera != player.Targeting.SubTarget.TargetCamera)
          {
            offset = new Vector3(0, 1, 1.5f);
            lockOnShotType.LockOn.Offset = offset;
            CameraManager.Instance.UseCamera(player.Targeting.SubTarget.TargetCamera);
          }
        }
        else if (player.Targeting.cameraLockTarget && !string.IsNullOrEmpty(player.Targeting.TargetId.Value))
        {
          lockOnShotType = player.Targeting.Target.lockOnShotType;
          if (lockOnShotType != null && MainCamera.Transition.CurrentShotCamera == PlayerCamera)
          {
            if (shotType.IsActive)
              shotType.OnDisable(MainCamera);
            if (!lockOnShotType.IsActive)
              lockOnShotType.OnEnable(MainCamera);
          }
          if (CameraManager.Instance.MainCamera.Transition.CurrentShotCamera != player.Targeting.Target.TargetCamera)
          {
            offset = new Vector3(0, 1, 1.5f);
            lockOnShotType.LockOn.Offset = offset;
            CameraManager.Instance.UseCamera(player.Targeting.Target.TargetCamera);
          }
        }
        else if (shotType != null)
        {
          if (lockOnShotType != null && lockOnShotType.IsActive)
            lockOnShotType.OnDisable(MainCamera);
          if (!shotType.IsActive)
            shotType.OnEnable(MainCamera);
          if (CameraManager.Instance.MainCamera.Transition.CurrentShotCamera != CameraManager.Instance.PlayerCamera)
          {
            lockOnShotType = null;
            CameraManager.Instance.UseCamera(CameraManager.Instance.PlayerCamera);
          }
        }
        else
        {
          if (lockOnShotType.IsActive)
            lockOnShotType.OnDisable(MainCamera);
          if (shotType.IsActive)
            shotType.OnDisable(MainCamera);
          CameraManager.Instance.UseCamera(CameraManager.Instance.LoginCamera);
        }
      }
      MoveTargetCamera();
    }

    void MoveTargetCamera()
    {
      if (LookAction == null || ZoomAction == null) return;
      // alter camera orthographic size if player shot is active
      if (lockOnShotType != null)
      {
        var look = LookAction.ReadValue<Vector2>().normalized;
        var zoom = ZoomAction.ReadValue<Vector2>().normalized;
        var value = look + zoom;
        if (!value.Equals(Vector2.zero))
        {
          offset = new Vector3(
            Mathf.Clamp(offset.x - look.x * Time.deltaTime * 8, -lockOnLimit.x, lockOnLimit.x),
            Mathf.Clamp(offset.y + look.y * Time.deltaTime * 8, -lockOnLimit.y, lockOnLimit.y),
            Mathf.Clamp(offset.z + zoom.y * Time.deltaTime * 8, -lockOnLimit.z, lockOnLimit.z)
          );

          lockOnShotType.LockOn.Offset = offset;
        }
      }

    }
  }
}