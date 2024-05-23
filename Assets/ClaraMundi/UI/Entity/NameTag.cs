using System;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
  public class NameTag : MonoBehaviour
  {
    public TextMeshProUGUI Text;
    public Transform Scaler;
    private Transform t;
    private Transform cameraTransform;
    private Entity entity;

    private void Awake()
    {
      entity = GetComponentInParent<Entity>();
      t = transform;
    }

    private void Update()
    {
      if (cameraTransform == null && CameraManager.Instance != null && CameraManager.Instance.Camera != null)
        cameraTransform = CameraManager.Instance.Camera.transform;
      if (entity && Text.text != entity.entityName.Value)
        Text.text = entity.entityName.Value;
      if (cameraTransform == null) return;
      t.rotation = cameraTransform.rotation;
      var distance = Mathf.Clamp(Vector3.Distance(cameraTransform.position, t.position), 0.1f, 100) / 4;
      var scale = (CameraManager.Instance.Camera.orthographicSize / 8) * distance;
      Scaler.transform.localScale = new Vector3(scale, scale, scale);
    }
  }
}