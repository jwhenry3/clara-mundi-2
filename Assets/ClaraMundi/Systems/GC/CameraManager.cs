using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;

namespace ClaraMundi
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        public MainCamera MainCamera;
        public ShotCamera LoginCamera;
        public ShotCamera PlayerCamera;
        public Camera Camera;
        

        private void Awake()
        {
            Instance = this;
            Camera = MainCamera.GetComponent<Camera>();
        }

        public void UsePlayerCamera()
        {
            MainCamera.Transition.CurrentShotCamera = PlayerCamera;
        }

        public void UseLoginCamera()
        {
            MainCamera.Transition.CurrentShotCamera = LoginCamera;
        }
    }
}