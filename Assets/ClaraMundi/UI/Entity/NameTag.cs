﻿using System;
using TMPro;
using UnityEngine;

namespace ClaraMundi
{
    public class NameTag : MonoBehaviour
    {
        public TextMeshProUGUI Text;
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
            if (entity && Text.text != entity.entityName)
                Text.text = entity.entityName;
            if (cameraTransform == null) return;
            t.rotation = cameraTransform.rotation;
            var scale = CameraManager.Instance.Camera.orthographicSize / 8;
            Text.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}