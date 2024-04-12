using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
    public class GameWindowHandler : MonoBehaviour
    {
        public static GameWindowHandler Instance;

        public Tabs Tabs;
        public MoveToFront ActiveWindow;

        private void Awake()
        {
            Instance = this;
            Tabs = GetComponent<Tabs>();
        }

        private void OnEnable()
        {
            InputManager.Instance.UI.FindAction("Inventory").performed += OnInventory;
            InputManager.Instance.UI.FindAction("Journal").performed += OnJournal;
            InputManager.Instance.UI.FindAction("Equipment").performed += OnEquipment;
            InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
        }

        private void OnDisable()
        {
            InputManager.Instance.UI.FindAction("Inventory").performed -= OnInventory;
            InputManager.Instance.UI.FindAction("Journal").performed -= OnJournal;
            InputManager.Instance.UI.FindAction("Equipment").performed -= OnEquipment;
            InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
        }


        private void OnInventory(InputAction.CallbackContext context)
        {
            if (InputManager.IsFocusedOnInput()) return;
            Tabs.ChangeTab("Inventory");
        }

        private void OnJournal(InputAction.CallbackContext context)
        {
            if (InputManager.IsFocusedOnInput()) return;
            Tabs.ChangeTab("Journal");
        }

        private void OnEquipment(InputAction.CallbackContext context)
        {
            if (InputManager.IsFocusedOnInput()) return;
            Tabs.ChangeTab("Equipment");
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (InputManager.IsFocusedOnInput()) return;
            if (ActiveWindow == null) return;
            var tab = Tabs.List.Find((t) => t.Content == ActiveWindow.GetComponent<UIAnimator>());
            if (tab != null)
              Tabs.ChangeTab(tab.Label);
        }
    }
}