using System.Collections;
using System.Linq;
using ClaraMundi.Quests;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class GameWindowHandler : MonoBehaviour
  {
    public static GameWindowHandler Instance;

    public CanvasGroup MenuCanvasGroup;

    public Tabs Tabs;

    public GameObject Menu;

    private bool menuOpen;
    private void Awake()
    {
      Instance = this;
    }

    private void OnEnable()
    {
      if (ChatWindowUI.Instance == null) return;
      InputManager.Instance.UI.FindAction("Character").performed += OnCharacter;
      InputManager.Instance.UI.FindAction("Journal").performed += OnJournal;
      InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
      InputManager.Instance.UI.FindAction("OpenChat").performed += OnChat;
      ChatWindowUI.Instance.MoveSibling.SentToBack += OnPreviousMenu;
    }

    private void OnDisable()
    {
      if (ChatWindowUI.Instance == null) return;
      Tabs.ChangeTab(""); // clear active tab so when we open the menu again it does not open the last opened tab
      InputManager.Instance.UI.FindAction("Character").performed -= OnCharacter;
      InputManager.Instance.UI.FindAction("Journal").performed -= OnJournal;
      InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
      InputManager.Instance.UI.FindAction("OpenChat").performed -= OnChat;
      ChatWindowUI.Instance.MoveSibling.SentToBack -= OnPreviousMenu;
    }


    private void OnCharacter(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToBack();
      Menu.SetActive(true);
      Tabs.ChangeTab("Character");
    }

    private void OnJournal(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToBack();
      Menu.SetActive(true);
      Tabs.ChangeTab("Journal");
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
      if (ChatWindowUI.Instance.MoveSibling.IsInFront()) return;
      // Debug.Log("Chat not in front");
      if (Tabs.CurrentTab == "")
      {
        // Debug.Log("Close Menu");
        Menu.SetActive(false);
        return;
      }
      // Debug.Log("Close Sub Menu");
      Tabs.ChangeTab("");
      foreach (var data in Tabs.List)
      {
        if (data.Button.GetComponent<AutoFocus>() != null)
        {
          EventSystem.current.SetSelectedGameObject(data.Button.gameObject);
          return;
        }
      }
    }

    public void OnPreviousMenu()
    {
      if (Menu.activeInHierarchy)
      {
        if (Tabs.CurrentTabData?.ContentForm != null)
        {
          EventSystem.current.SetSelectedGameObject(Tabs.CurrentTabData.ContentForm.gameObject);
        }
        else
          Menu.SetActive(false);
      }
    }

    void Update()
    {
      bool chatInFront = ChatWindowUI.Instance.MoveSibling.IsInFront();
      bool isMenuOpen = Menu.activeInHierarchy || chatInFront;

      if (isMenuOpen && !menuOpen)
        InputManager.Instance.World.Disable();
      else if (!isMenuOpen && menuOpen)
        InputManager.Instance.World.Enable();
      menuOpen = isMenuOpen;

      MenuCanvasGroup.interactable = Menu.activeInHierarchy && !chatInFront && Tabs.CurrentTab == "";
    }

    public void OnChat(InputAction.CallbackContext context)
    {
      ChatWindowUI.Instance.MoveSibling.ToFront();
      EventSystem.current.SetSelectedGameObject(ChatWindowUI.Instance.InputField.gameObject);
      StartCoroutine(FocusChatInput());
    }

    public IEnumerator FocusChatInput()
    {
      yield return new WaitForSeconds(0.5f);
      ChatWindowUI.Instance.InputField.ActivateInputField();
    }
  }
}