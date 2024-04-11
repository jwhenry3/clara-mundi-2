using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class ClickToMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
  {
    private bool isPressing;
    private float updateTick;
    private Ray cameraRay;

    private LayerMask mask;
    private RaycastHit hit;

    void Start() {
      mask = LayerMask.GetMask(new string[] { LayerMask.LayerToName(gameObject.layer) });
    }
    private void Update()
    {
      if (!isPressing) return;
      if (PlayerManager.Instance.LocalPlayer == null) return;
      if (updateTick > 0)
        updateTick -= Time.deltaTime;
      if (updateTick <= 0)
      {
        updateTick = 0;
        if (GetClickedPosition(out Vector3 worldPosition))
          PlayerManager.Instance.LocalPlayer.Movement.UpdateDestination(worldPosition);
      }
    }

    private bool GetClickedPosition(out Vector3 worldPosition)
    {
      Vector2 pointerPosition = new(Pointer.current.position.x.value, Pointer.current.position.y.value);
      cameraRay = Camera.main.ScreenPointToRay(pointerPosition);
      bool raycastDidHit = Physics.Raycast(cameraRay, out hit, 100, mask);
      worldPosition = hit.point;
      return raycastDidHit;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
      if (PlayerManager.Instance.LocalPlayer == null) return;
      if (eventData.button != PointerEventData.InputButton.Left) return;
      isPressing = true;
      Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (eventData.button != PointerEventData.InputButton.Left) return;
      isPressing = false;
      Cursor.lockState = CursorLockMode.None;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
      if (!isPressing || updateTick != 0) return;
      updateTick = 0.1f;
    }
  }
}