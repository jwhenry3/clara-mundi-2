using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
  public class LocalTrailRenderer : MonoBehaviour
  {
    private TrailRenderer trailRenderer;

    private Vector3 lastFramePosition;

    private void Start()
    {
      trailRenderer = GetComponent<TrailRenderer>();
      lastFramePosition = transform.parent.position;
    }

    private void LateUpdate()
    {
      var delta = transform.parent.position - lastFramePosition;
      lastFramePosition = transform.parent.position;

      var positions = new Vector3[trailRenderer.positionCount];
      trailRenderer.GetPositions(positions);

      for (var i = 0; i < trailRenderer.positionCount; i++)
      {
        positions[i] += delta;
      }

      trailRenderer.SetPositions(positions);
    }
  }
}