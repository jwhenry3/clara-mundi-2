using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public GameObject tooltip;

        private void Awake()
        {
            tooltip = transform.Find("Tooltip")?.gameObject;
            if (tooltip == null) return;
            tooltip.SetActive(false);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip == null) return;
            tooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip == null) return;
            tooltip.SetActive(false);
        }
    }
}