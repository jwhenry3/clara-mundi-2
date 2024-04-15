using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class InactiveOpacity : MonoBehaviour
  {
    public CanvasGroup canvasGroup;
    public float Opacity = 0.75f;
    void Update()
    {
      if (canvasGroup.interactable)
        canvasGroup.alpha = 1;
      else
        canvasGroup.alpha = Opacity;
    }
  }
}