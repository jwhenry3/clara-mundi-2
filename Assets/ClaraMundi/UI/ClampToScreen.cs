using UnityEngine;

namespace ClaraMundi
{
  public class ClampToScreen : MonoBehaviour
  {
    public RectTransform CanvasRect;
    public RectTransform PanelRect;
    private void LateUpdate()
    {
      if (CanvasRect != null)
        UpdateCanvas();
    }

    void UpdateCanvas()
    {

      var sizeDelta = CanvasRect.sizeDelta - PanelRect.sizeDelta;
      var panelPivot = PanelRect.pivot;
      var position = PanelRect.anchoredPosition;
      position.x = Mathf.Clamp(position.x, -sizeDelta.x * panelPivot.x, sizeDelta.x * (1 - panelPivot.x));
      position.y = Mathf.Clamp(position.y, -sizeDelta.y * panelPivot.y, sizeDelta.y * (1 - panelPivot.y));
      PanelRect.anchoredPosition = position;
    }
  }
}