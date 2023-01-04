using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class ClickToMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        private bool isPressing;
        private Vector3 currentPosition;
        private float updateTick;

        private void Update()
        {
            updateTick += Time.deltaTime;
            if (updateTick < 1) return;
            updateTick = 0;
            if (isPressing && currentPosition != Vector3.zero)
                PlayerManager.Instance.LocalPlayer.Movement.UpdateDestination(currentPosition);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPressing = true;
            PlayerManager.Instance.LocalPlayer.Movement.UpdateDestination(eventData.pointerCurrentRaycast.worldPosition);
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPressing = false;
            currentPosition = Vector3.zero;
            Cursor.lockState = CursorLockMode.None;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isPressing)
                currentPosition = eventData.pointerCurrentRaycast.worldPosition;
        }
    }
}