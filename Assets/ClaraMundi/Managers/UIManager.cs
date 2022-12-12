using UnityEngine;

namespace ClaraMundi
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public UIAnimator InventoryAnimator;
        public RectTransform Backdrop;

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
    }
}