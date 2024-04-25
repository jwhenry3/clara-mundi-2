using System;
using FishNet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi.Testing
{
  public class CameraMover : MonoBehaviour
  {
    public InputActionAsset InputActionAsset;

    public float speed = 5;
    public Transform DirectionTransform;

    private InputAction moveAction;
    private Transform t;
    private Camera c;

    private Vector3 moveDirection;
    void OnEnable()
    {
      t = transform;
      c = GetComponent<Camera>();
      moveAction = InputActionAsset.FindAction("Player/Move");
    }

    private void Update()
    {
      if (InstanceFinder.NetworkManager.IsServerStarted)
      {
        if (!c.enabled)
        {
          c.enabled = true;
        }
        var move = Vector3.zero;
        var input = moveAction.ReadValue<Vector2>();
        var forward = DirectionTransform.forward;
        var right = DirectionTransform.right;
        move.z = input.y;
        move.x = input.x;
        move.Normalize();
        var desiredMoveDirection = forward * move.z + right * move.x;

        t.position += desiredMoveDirection * speed * Time.deltaTime;
      }
    }
  }
}