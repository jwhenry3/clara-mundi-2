using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    
    public class ContextMenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ContextMenuItemData Data;
        public TextMeshProUGUI Label;
        public GameObject Background;

        public void OnPointerClick(PointerEventData eventData)
        {
            Data.OnClick?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Background.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Background.SetActive(false);
        }
    }
}