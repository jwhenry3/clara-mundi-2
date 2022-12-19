using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public UIAnimator InventoryAnimator;
        public RectTransform Backdrop;
        public MoveToFront ActiveWindow;

        private void Awake()
        {
            Instance = this;
        }

        public void ToggleInventory()
        {
            Backdrop.gameObject.SetActive(InventoryAnimator.IsHidden());
            InventoryAnimator.Toggle();
        }

        public void CloseAll()
        {
            Backdrop.gameObject.SetActive(false);
            InventoryAnimator.Hide();
        }

        public void LateUpdate()
        {
            if (EventSystem.current.currentSelectedGameObject != null) return;
            if (ActiveWindow  == null) return;
            if (ActiveWindow.gameObject.activeInHierarchy)
                ActiveWindow.SelectFirstInteractable();
        }
    }
}