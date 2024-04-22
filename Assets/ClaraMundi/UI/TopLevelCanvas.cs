using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace ClaraMundi
{

  [Serializable]
  public class ControlsDictionary : UnitySerializedDictionary<string, CanvasGroupFocus> { }
  public class TopLevelCanvas : MonoBehaviour
  {

    public static TopLevelCanvas Instance;
    public bool IsDebug;
    public GameObject Container;
    public InputActionAsset InputActionAsset;
    private InputActionMap World;

    public ControlsDictionary Controls;
    void Start()
    {
      Instance = this;
      Container.SetActive(IsDebug);
      World = InputActionAsset.FindActionMap("Player");
    }
    void OnEnable()
    {
      InputActionAsset.FindAction("UI/Navigate").performed += OnSelect;
      InputActionAsset.FindAction("UI/NextElement").performed += OnSelect;
      InputActionAsset.FindAction("UI/Cancel").performed += OnCancel;
      InputActionAsset.FindAction("UI/Menu").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Character").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Skills").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Inventory").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Party").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Journal").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Map").performed += Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Character").performed += Controls["Character"].Show;
      InputActionAsset.FindAction("UI/Skills").performed += Controls["Skills"].Show;
      InputActionAsset.FindAction("UI/Inventory").performed += Controls["Inventory"].Show;
      InputActionAsset.FindAction("UI/Party").performed += Controls["Party"].Show;
      InputActionAsset.FindAction("UI/Journal").performed += Controls["Journal"].Show;
      InputActionAsset.FindAction("UI/Map").performed += Controls["Map"].Show;

    }

    void OnDisable()
    {
      InputActionAsset.FindAction("UI/Navigate").performed -= OnSelect;
      InputActionAsset.FindAction("UI/NextElement").performed -= OnSelect;
      InputActionAsset.FindAction("UI/Cancel").performed -= OnCancel;
      InputActionAsset.FindAction("UI/Menu").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Character").performed -= Controls["Character"].Show;
      InputActionAsset.FindAction("UI/Character").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Skills").performed -= Controls["Skills"].Show;
      InputActionAsset.FindAction("UI/Skills").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Inventory").performed -= Controls["Inventory"].Show;
      InputActionAsset.FindAction("UI/Inventory").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Party").performed -= Controls["Party"].Show;
      InputActionAsset.FindAction("UI/Party").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Journal").performed -= Controls["Journal"].Show;
      InputActionAsset.FindAction("UI/Journal").performed -= Controls["Menu"].Show;
      InputActionAsset.FindAction("UI/Map").performed -= Controls["Map"].Show;
      InputActionAsset.FindAction("UI/Map").performed -= Controls["Menu"].Show;
    }
    public void OnSelect(InputAction.CallbackContext eventData)
    {
      if (EventSystem.current.currentSelectedGameObject != null) return;
      if (ButtonWithHybridNav.LastButton != null)
        EventSystem.current.SetSelectedGameObject(ButtonWithHybridNav.LastButton.gameObject);
      else if (InputFieldWithHybridNav.LastInput != null)
        EventSystem.current.SetSelectedGameObject(InputFieldWithHybridNav.LastInput.gameObject);
      else
      {
        // find first active UI section and select the first relevant element if possible
        foreach (CanvasGroupFocus control in Controls.Values)
        {
          if (control.gameObject.activeInHierarchy && control.canvasGroup.interactable)
          {
            control.Select();
            return;
          }
        }
      }
    }
    public void Update()
    {
      if (World == null) return;
      bool shouldDisableWorldInput = Controls["Menu"].gameObject.activeInHierarchy;
      if (World.enabled && shouldDisableWorldInput)
        World.Disable();
      else if (!World.enabled && !shouldDisableWorldInput)
        World.Enable();


      if (IsDebug) return;
      bool shouldEnableUI = PlayerManager.Instance?.LocalPlayer != null;
      if (Container.activeSelf != shouldEnableUI)
        Container.SetActive(shouldEnableUI);
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (Controls["Menu"].gameObject.activeInHierarchy)
      {
        if (Controls["Menu"].canvasGroup.interactable)
          Controls["Menu"].gameObject.SetActive(false);
      }
    }
  }
}