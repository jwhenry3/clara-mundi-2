using System;
using UnityEngine;

namespace ClaraMundi.Testing
{
    public class CameraMover : MonoBehaviour
    {
        private Transform t;
        private Camera c;
        private Transform cameraTransform;
        public float speed = 5;
        private void Start()
        {
            t = transform;
            c = Camera.main;
            cameraTransform = c!.transform;
        }

        private void Update()
        {
            var move = Vector3.zero;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
            move.z = Input.GetAxis("Vertical");
            move.x = Input.GetAxis("Horizontal");
            move.Normalize();
            var desiredMoveDirection = forward * move.z + right * move.x;
            t.Translate(move * speed * Time.deltaTime);
        }
    }
}