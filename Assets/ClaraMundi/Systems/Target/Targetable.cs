using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ClaraMundi
{
  public class Targetable : MonoBehaviour, IComparable
  {
    public Entity Entity;
    public TargetController TargetController;
    public bool DistanceFirst = true;

    public float DistanceFromCamera;
    public float HorizontalPosition;
    public float VerticalPosition;

    public float HorizontalPercent;
    public float VerticalPercent;

    public float Score;

    public bool OnScreen;

    private Camera cam;
    private Transform cameraTransform;


    private float updateInterval = 0.5f;
    private float currentTick = 0;

    public void Start()
    {
      cam = Camera.main;
      cameraTransform = cam.transform;
    }

    public int CompareTo(object obj)
    {
      var a = this;
      var b = obj as Targetable;

      if (a.Score > b.Score)
        return -1;

      if (a.Score < b.Score)
        return 1;

      return 0;
    }

    public void LateUpdate()
    {
      // server has no need for this information
      // and we only want to calculate when the local player is present
      if (PlayerManager.Instance?.LocalPlayer == null) return;
      currentTick += Time.deltaTime;
      if (currentTick > updateInterval)
      {
        currentTick = 0;
        UpdateDetails();
      }
    }

    private void UpdateDetails()
    {
      DistanceFromCamera = Vector3.Distance(transform.position, cameraTransform.position);
      Vector3 screenPoint = cam.WorldToScreenPoint(transform.position);
      HorizontalPosition = screenPoint.x;
      VerticalPosition = screenPoint.y;
      HorizontalPercent = screenPoint.x / Screen.width;
      if (DistanceFirst)
        Score = DistanceFromCamera * 100_000 + (HorizontalPercent * 100);
      else
        Score = HorizontalPercent * 100_000 + DistanceFromCamera;
      OnScreen = HorizontalPosition > 0 && HorizontalPosition < Screen.width && VerticalPosition > 0 && VerticalPosition < Screen.height;
    }
  }
}