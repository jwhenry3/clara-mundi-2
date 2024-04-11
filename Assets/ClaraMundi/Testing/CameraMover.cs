using System;
using FishNet;
using Unity.VisualScripting;
using UnityEngine;

namespace ClaraMundi.Testing
{
    public class CameraMover : MonoBehaviour
    {
        public float speed = 5;
        public Transform Transform;
        private Transform t;
        private Camera c;
        void OnEnable() {
          t = transform;
          c = GetComponent<Camera>();
        }
        private void Update()
        {
          if (InstanceFinder.NetworkManager.IsServerStarted) {
            if (!c.enabled) {
              c.enabled = true;
            }
            var move = Vector3.zero;
            var forward = Transform.forward;
            var right = Transform.right;
            move.z = Input.GetAxis("Vertical");
            move.x = Input.GetAxis("Horizontal");
            move.Normalize();
            var desiredMoveDirection = forward * move.z + right * move.x;
            
            t.position += desiredMoveDirection * speed * Time.deltaTime;
          }
        }
    }
}