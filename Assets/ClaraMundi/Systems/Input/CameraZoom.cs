using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
    public class CameraZoom : MonoBehaviour
    {

        public InputActionAsset InputActionAsset;
        public ShotCamera PlayerCamera;
        public MainCamera MainCamera;
        private Camera Camera;

        private InputAction InputAction;

        private float zoomValue;

        private void Awake()
        {
            Camera = MainCamera.GetComponent<Camera>();
            InputAction = InputActionAsset.FindActionMap("Player").FindAction("Look");
            InputAction.performed += OnPerform;
        }

        private void OnDestroy()
        {
            InputAction.performed -= OnPerform;
        }

        void OnPerform(InputAction.CallbackContext context)
        {
            // alter camera orthographic size if player shot is active
            if (MainCamera.Transition.CurrentShotCamera == PlayerCamera)
            {
                zoomValue += context.ReadValue<float>() / 4;
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