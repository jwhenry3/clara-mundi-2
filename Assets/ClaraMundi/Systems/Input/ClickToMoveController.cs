﻿using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameCreator.Runtime.Common;
using UnityEngine;
using GCharacter = GameCreator.Runtime.Characters.Character;

namespace ClaraMundi
{
  public class ClickToMoveController : PlayerController
  {
    private readonly SyncVar<Vector3> destination = new(new SyncTypeSettings(WritePermission.ClientUnsynchronized));

    private Vector3 lastDestination;
    public bool debugLog;

    private void Update()
    {
      if (lastDestination != destination.Value)
        MovePlayerTo(destination.Value);
      lastDestination = destination.Value;
    }

    public void UpdateDestination(Vector3 dest)
    {
      if (!destination.Value.Equals(dest))
        SendDestination(dest);
    }

    [ServerRpc(RunLocally = true)]
    private void SendDestination(Vector3 dest)
    {
      if (!destination.Value.Equals(dest))
        destination.Value = dest;
    }

    private void MovePlayerTo(Vector3 dest)
    {
      player.Body.Motion.MoveToLocation(new Location(dest), 0.0f, OnReachedDestination, 1);
      if (IsServerStarted) return;
      if (!debugLog) return;
      ChatManager.ReceivedMessage(new ChatMessage()
      {
        Type = ChatMessageType.System,
        Message = $"Moving to ({Round(dest.x)}, {Round(dest.y)}, {Round(dest.z)})..."
      });
    }

    private float Round(float value)
    {
      return Mathf.Round(value * 100) / 100;
    }

    private void OnReachedDestination(GCharacter character, bool condition)
    {
      if (IsServerStarted) return;
      if (!debugLog) return;
      ChatManager.ReceivedMessage(new ChatMessage()
      {
        Type = ChatMessageType.System,
        Message = "Reached Destination!"
      });
    }
  }
}