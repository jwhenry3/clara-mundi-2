using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class GameWindowHandler : MonoBehaviour
  {
    public static GameWindowHandler Instance;

    public Tabs Tabs;

    public GameObject Menu;

    private bool menuOpen;
    private void Awake()
    {
      Instance = this;
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
      Menu.SetActive(true);
    }

    private void OnJournal(InputAction.CallbackContext context)
    {
      if (InputManager.IsFocusedOnInput()) return;
      Tabs.ChangeTab("Journal");
      Menu.SetActive(true);
    }

    private void OnEquipment(InputAction.CallbackContext context)
    {
      if (InputManager.IsFocusedOnInput()) return;
      Tabs.ChangeTab("Equipment");
      Menu.SetActive(true);
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
      if (Tabs.CurrentTab == "")
      {
        Menu.SetActive(false);
        return;
      }
      Tabs.ChangeTab(Tabs.CurrentTab);
      Tabs.Form.PreviouslySelected?.Activate();
    }

    void Update()
    {
      if (Menu.activeInHierarchy && !menuOpen)
      {
        InputManager.Instance.World.Disable();
      }
      else if (!Menu.activeInHierarchy && menuOpen)
      {
        InputManager.Instance.World.Enable();
      }
      menuOpen = Menu.activeInHierarchy;
    }
  }
}