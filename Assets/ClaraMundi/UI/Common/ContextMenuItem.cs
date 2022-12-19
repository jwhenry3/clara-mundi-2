using System;
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
            Activate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Background == null) return;
            Background.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Background == null) return;
            Background.SetActive(false);
        }

        public void Activate()
        {
            Data.OnClick?.Invoke();
            if (Background == null) return;
            Background.SetActive(false);
        }
    }
}