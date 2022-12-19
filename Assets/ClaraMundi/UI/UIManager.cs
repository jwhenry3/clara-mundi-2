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
        public RectTransform WindowsContainer;
        public List<MoveToFront> Windows;

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

        private float updateTick = 0;

        public void Update()
        {
            updateTick += Time.deltaTime;
            if (!(updateTick > 1)) return;
            updateTick = 0;
            if (EventSystem.current.currentSelectedGameObject != null) return;
            var last = Windows.Find((w) =>
            {
                if (w.Parent == null) return false;
                return w.Parent.GetSiblingIndex() == WindowsContainer.childCount - 1;
            });
            if (last == null) return;
            if (last.gameObject.activeInHierarchy)
                last.SelectFirstInteractable();
        }
    }
}