using System;
using GameCreator.Runtime.Cameras;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace ClaraMundi
{
  public class Targetable : MonoBehaviour, IComparable
  {
    public Entity Entity;
    public TargetController TargetController;
    public GameObject SubTargetIndicator;
    public Transform TargetIndicatorPosition;

    public float IndicatorRadius = 0.5f;
    public bool DistanceFirst = false;

    public TextMeshProUGUI NameTag;

    public float DistanceFromCamera;
    public float HorizontalPosition;
    public float VerticalPosition;

    public float HorizontalPercent;
    public float VerticalPercent;
    public float CenterScore;

    public float Score;

    public bool OnScreen;

    private Camera cam;
    private Transform cameraTransform;

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

    void Update()
    {
      if (TargetController != null)
      {
        SubTargetIndicator?.SetActive(TargetController.SubTarget == this);
        if (NameTag != null)
          NameTag.gameObject.SetActive(TargetController.SubTarget == this || TargetController.Target == this);
      }
      else
      {
        SubTargetIndicator?.SetActive(false);
        if (NameTag != null)
          NameTag.gameObject.SetActive(false);
      }
    }


    public void UpdateDetails()
    {
      DistanceFromCamera = Vector3.Distance(transform.position, cameraTransform.position);
      Vector3 screenPoint = cam.WorldToScreenPoint(transform.position);
      HorizontalPosition = screenPoint.x;
      VerticalPosition = screenPoint.y;
      HorizontalPercent = screenPoint.x / Screen.width;
      CenterScore = 0.50f - Mathf.Abs(HorizontalPercent - .50f);
      if (DistanceFirst)
        Score = DistanceFromCamera * 100_000 + (HorizontalPercent * 100);
      else
        Score = HorizontalPercent * 100_000 + DistanceFromCamera;
      OnScreen = HorizontalPosition > 0 && HorizontalPosition < Screen.width && VerticalPosition > 0 && VerticalPosition < Screen.height;
    }
  }
}