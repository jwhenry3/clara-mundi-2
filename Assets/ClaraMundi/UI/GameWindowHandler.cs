using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static ClaraMundi.InputManager;

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

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != null) return;
            if (ActiveWindow  == null) return;
            if (ActiveWindow.gameObject.activeInHierarchy)
                ActiveWindow.SelectFirstInteractable();
        }

        private void OnInventory(InputAction.CallbackContext context)
        {
            if (InputManager.Instance.InputsFocused.Count == 0) Tabs.ChangeTab("Inventory");
        }

        private void OnJournal(InputAction.CallbackContext context) 
        {
            if (InputManager.Instance.InputsFocused.Count == 0) Tabs.ChangeTab("Journal");
        }
        private void OnEquipment(InputAction.CallbackContext context) 
        {
            if (InputManager.Instance.InputsFocused.Count == 0) Tabs.ChangeTab("Equipment");
        }
        private void OnCancel(InputAction.CallbackContext context)
        {
            if (InputManager.Instance.InputsFocused.Count != 0) return;
            if (ActiveWindow == null) return;
            var tab = Tabs.List.Find((t) => t.Content == ActiveWindow.GetComponent<UIAnimator>());
            Tabs.ChangeTab(tab.Label);
        }
    }
}