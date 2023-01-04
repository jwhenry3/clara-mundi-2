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
        

        private void Awake()
        {
            Instance = this;
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