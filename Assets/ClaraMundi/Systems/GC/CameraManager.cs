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
    public Transform CameraTransform;


    private void Awake()
    {
      Instance = this;
      Camera = MainCamera.GetComponent<Camera>();
      CameraTransform = Camera.transform;
    }

    public void UsePlayerCamera(Player player)
    {
      MainCamera.Transition.CurrentShotCamera = PlayerCamera;

    }
    public void UseCamera(ShotCamera camera)
    {
      MainCamera.Transition.CurrentShotCamera = camera;
    }

    public void UseLoginCamera()
    {
      MainCamera.Transition.CurrentShotCamera = LoginCamera;
    }
  }
}