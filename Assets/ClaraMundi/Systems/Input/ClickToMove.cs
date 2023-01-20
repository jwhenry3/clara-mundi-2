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

        private PointerEventData lastEvent;

        private void Update()
        {
            if (PlayerManager.Instance.LocalPlayer == null) return;
            if (updateTick > 0)
                updateTick -= Time.deltaTime;
            if (updateTick <= 0)
            {
                updateTick = 0;
                if (lastEvent != null)
                    PlayerManager.Instance.LocalPlayer.Movement.UpdateDestination(lastEvent.pointerCurrentRaycast.worldPosition);
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (PlayerManager.Instance.LocalPlayer == null) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPressing = true;
            lastEvent = eventData;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPressing = false;
            lastEvent = null;
            Cursor.lockState = CursorLockMode.None;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!isPressing || updateTick != 0) return;
            updateTick = 0.5f;
            lastEvent = eventData;
        }
    }
}