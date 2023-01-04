using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameCreator.Runtime.Common;
using UnityEngine;
using GCharacter = GameCreator.Runtime.Characters.Character;

namespace ClaraMundi
{
    public class ClickToMoveController : PlayerController
    {
        private Camera camera;
        [SyncVar] private Vector3 destination;

        private Vector3 lastDestination;

        public override void OnStartClient()
        {
            base.OnStartClient();
            camera = CameraManager.Instance.MainCamera.GetComponent<Camera>();
        }

        private float triggeredCooldown;
        private void Update()
        {
            if (!IsServer)
                HandleClick();
            if (lastDestination != destination)
                MovePlayerTo(destination);
            lastDestination = destination;
            if (triggeredCooldown > 0)
                triggeredCooldown = Mathf.Max(0, triggeredCooldown - Time.deltaTime);
        }

        private void HandleClick()
        {
            
            bool clicked = triggeredCooldown == 0 && Input.GetMouseButton(0);

            if (!clicked) return;
            triggeredCooldown = 0.5f;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            if (!hit.transform.CompareTag("Ground")) return;
            SendDestination(hit.point);
        }

        [ServerRpc(RunLocally = true)]
        private void SendDestination(Vector3 dest)
        {
            destination = dest;
        }

        private void MovePlayerTo(Vector3 dest)
        {
            player.Body.Motion.MoveToLocation(new Location(dest), 0.0f, OnReachedDestination, 1);
            if (IsServer) return;
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

        private void OnReachedDestination(GCharacter character)
        {
            if (IsServer) return;
            ChatManager.ReceivedMessage(new ChatMessage()
            {
                Type = ChatMessageType.System,
                Message = "Reached Destination!"
            });
        }
    }
}