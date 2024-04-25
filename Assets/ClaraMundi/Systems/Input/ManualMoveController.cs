using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using GCharacter = GameCreator.Runtime.Characters.Character;

namespace ClaraMundi
{
  public class ManualMoveController : PlayerController
  {

    private InputAction InputAction;
    private readonly SyncVar<Vector3> direction = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));
    private readonly SyncVar<Vector3> lookDirection = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));

    private Vector3 lastDirection;
    public bool debugLog;

    private readonly SyncVar<Vector3> position = new();

    private void Update()
    {
      if (IsClientStarted && IsOwner)
      {
        if (InputAction == null && InputManager.Instance != null && InputManager.Instance.World != null)
          InputAction = InputManager.Instance.World.FindAction("Move");
        if (InputAction != null)
          UpdateDirection(InputAction.ReadValue<Vector2>().normalized);
      }
      if (IsClientStarted && !position.Value.Equals(player.transform.position))
        SynchronizePosition();
      if (!lastDirection.Equals(direction.Value) || !direction.Value.Equals(Vector3.zero))
        MovePlayerTo(direction.Value);
      lastDirection = direction.Value;
      if (IsServerStarted)
      {
        if (!position.Value.Equals(player.transform.position))
        {
          position.Value = player.transform.position;
        }
      }
    }

    void SynchronizePosition()
    {
      var dir = position.Value - player.transform.position;
      if (dir.magnitude > 0.1f)
      {
        player.transform.position += dir * 4 * Time.deltaTime; // interpolate at a slowish rate, some difference is acceptable
      }
    }

    public void UpdateDirection(Vector2 dir)
    {
      dir = dir.normalized;
      var forward = CameraManager.Instance.CameraTransform.forward;
      var right = CameraManager.Instance.CameraTransform.right;
      Vector3 desiredMoveDirection = forward * dir.y + right * dir.x;
      if (!direction.Value.Equals(desiredMoveDirection))
        SendDirection(desiredMoveDirection);
    }

    [ServerRpc(RunLocally = true)]
    private void SendDirection(Vector3 dir)
    {
      dir = dir.normalized;
      if (!direction.Value.Equals(dir))
      {
        direction.Value = dir;
      }
    }

    private void MovePlayerTo(Vector3 dir)
    {
      if (dir.Equals(Vector3.zero))
      {
        player.Body.Motion.StopToDirection();
        Quaternion look = Quaternion.Euler(lookDirection.Value);
        UnitDriverNavmesh unit = player.Body.Driver as UnitDriverNavmesh;
        if (!IsOwner && unit.Transform.rotation != look)
          player.Body.Driver.SetRotation(look);
      }
      else
      {
        if (IsServerInitialized)
          lookDirection.Value = dir;
        player.Body.Motion.MoveToDirection(dir * player.Body.Motion.LinearSpeed, Space.World);
      }
      if (IsServerStarted) return;
      if (!debugLog) return;
      ChatManager.ReceivedMessage(new ChatMessage()
      {
        Type = ChatMessageType.System,
        Message = $"Moving in direction ({Round(dir.x)}, {Round(dir.y)})..."
      });
    }

    private float Round(float value)
    {
      return Mathf.Round(value * 100) / 100;
    }
  }
}