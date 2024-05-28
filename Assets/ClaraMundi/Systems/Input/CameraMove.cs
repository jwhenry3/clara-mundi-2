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

    private Vector3 lockOnLimit = new Vector3(3, 3, 3);
    private Vector3 offset = new Vector3(0, 1, 0);

    public Player player => PlayerManager.Instance != null ? PlayerManager.Instance.LocalPlayer : null;


    public Transform LockOnTransform;
    void OnEnable()
    {

      lockOnShotType = (ShotTypeLockOn)PlayerTargetCamera.ShotType;
      shotType = (ShotTypeThirdPerson)PlayerCamera.ShotType;
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
        var targetToLockOn = player.Targeting.SubTarget ?? player.Targeting.Target;
        if (targetToLockOn != null)
          LockOnTransform.position = targetToLockOn.transform.position;
        bool targetLock = string.IsNullOrEmpty(player.Targeting.SubTargetId.Value) && player.Targeting.CameraLockTarget.Value;
        bool subTargetLock = !string.IsNullOrEmpty(player.Targeting.SubTargetId.Value) && player.Targeting.CameraLockSubTarget.Value;
        if (targetLock || subTargetLock)
        {
          if (CameraManager.Instance.MainCamera.Transition.CurrentShotCamera != PlayerTargetCamera)
          {
            if (shotType.IsActive)
              shotType.OnDisable(MainCamera);
            if (!lockOnShotType.IsActive)
              lockOnShotType.OnEnable(MainCamera);
            offset = new Vector3(0, 1, 1.5f);
            lockOnShotType.LockOn.Offset = offset;
            CameraManager.Instance.UseCamera(PlayerTargetCamera);
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