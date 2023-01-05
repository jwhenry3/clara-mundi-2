using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
    public class CameraZoom : MonoBehaviour
    {
        public ShotCamera PlayerCamera;
        public MainCamera MainCamera;
        private Camera Camera;

        private InputAction InputAction;

        public float zoomValue = 5;

        private void Awake()
        {
            Camera = MainCamera.GetComponent<Camera>();
            InputAction = InputManager.Instance.World.FindAction("Zoom");
            InputAction.performed += OnPerform;
            Camera.orthographicSize = zoomValue;
        }

        private void OnDestroy()
        {
            InputAction.performed -= OnPerform;
        }

        void OnPerform(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>().normalized.y * 0.25f;
            // alter camera orthographic size if player shot is active
            if (MainCamera.Transition.CurrentShotCamera == PlayerCamera)
            {
                zoomValue = Mathf.Clamp(zoomValue - value, 0, 7);
                Camera.orthographicSize = zoomValue;
            }
            else
            {
                zoomValue = 5;
                Camera.orthographicSize = zoomValue;
            }
        }
    }
}