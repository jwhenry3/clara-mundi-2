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
      InputManager.Instance.UI.FindAction("OpenChat").performed += OnChat;
      ChatWindowUI.Instance.MoveSibling.SentToBack += OnPreviousMenu;
    }

    private void OnDisable()
    {
      Tabs.ChangeTab(""); // clear active tab so when we open the menu again it does not open the last opened tab
      InputManager.Instance.UI.FindAction("Inventory").performed -= OnInventory;
      InputManager.Instance.UI.FindAction("Journal").performed -= OnJournal;
      InputManager.Instance.UI.FindAction("Equipment").performed -= OnEquipment;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      InputManager.Instance.UI.FindAction("OpenChat").performed -= OnChat;
      ChatWindowUI.Instance.MoveSibling.SentToBack -= OnPreviousMenu;
    }


    private void OnInventory(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToBack();
      Menu.SetActive(true);
      Tabs.ChangeTab("Inventory");
    }

    private void OnJournal(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToBack();
      Menu.SetActive(true);
      Tabs.ChangeTab("Journal");
    }

    private void OnEquipment(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToBack();
      Menu.SetActive(true);
      Tabs.ChangeTab("Equipment");
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
      if (ChatWindowUI.Instance.MoveSibling.IsInFront())
      {
        ChatWindowUI.Instance.MoveSibling.ToBack();
        return;
      }
      if (Tabs.CurrentTab == "")
      {
        Menu.SetActive(false);
        return;
      }
      Tabs.ChangeTab(Tabs.CurrentTab);
      Tabs.Form.PreviouslySelected?.Activate();
    }

    public void OnPreviousMenu()
    {
      if (Menu.activeInHierarchy)
      {
        if (Tabs.CurrentTabData != null)
          Tabs.CurrentTabData.Content.GetComponent<Form>()?.PreviouslySelected?.Activate();
        else
          Menu.SetActive(false);
      }
    }

    void Update()
    {
      if ((Menu.activeInHierarchy && !menuOpen) || ChatWindowUI.Instance.MoveSibling.IsInFront())
      {
        InputManager.Instance.World.Disable();
      }
      else if (!Menu.activeInHierarchy && menuOpen)
      {
        InputManager.Instance.World.Enable();
      }
      menuOpen = Menu.activeInHierarchy;
    }

    public void OnChat(InputAction.CallbackContext context)
    {
      EventSystem.current.SetSelectedGameObject(ChatWindowUI.Instance.InputField.gameObject);
      ChatWindowUI.Instance.MoveSibling.ToFront();
    }
  }
}