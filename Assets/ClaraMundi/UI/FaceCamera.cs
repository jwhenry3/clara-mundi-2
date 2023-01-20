using System.Collections;
using ClaraMundi;
using TMPro;
using UnityEngine;

namespace Assets.ClaraMundi.UI
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform t;

        // Use this for initialization
        private void Start()
        {
            t = transform;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            t.LookAt(CameraManager.Instance.Camera.transform);
        }
    }
}