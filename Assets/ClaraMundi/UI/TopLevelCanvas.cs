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
  public class TopLevelCanvas : PlayerUI
  {
    public EventSystem EventSystem;
    public static TopLevelCanvas Instance;
    public bool IsDebug;
    public GameObject Container;
    public InputActionAsset InputActionAsset;
    private InputActionMap World;

    public ControlsDictionary Controls;

    public ChatWindowUI Chat;
    public override void Start()
    {
      base.Start();
      Instance = this;
      EventSystem.gameObject.SetActive(IsDebug);
      Container.SetActive(IsDebug);
      World = InputActionAsset.FindActionMap("Player");
    }

    protected override void OnPlayerChange(Player _player)
    {
      if (player != null)
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
      Debug.Log("Player Changed");
      base.OnPlayerChange(_player);
      Container.SetActive(IsDebug || _player != null);
      if (_player != null)
      {
        CloseAll();
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

    }

    void CloseAll()
    {
      foreach (var kvp in Controls)
        kvp.Value.gameObject.SetActive(false);
    }

    void OnDisable()
    {
      if (player != null)
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
      bool shouldDisableWorldInput = Controls["Menu"].gameObject.activeInHierarchy || InputFieldWithHybridNav.CurrentInput != null || ButtonWithHybridNav.CurrentButton != null;
      if (World.enabled && shouldDisableWorldInput)
        World.Disable();
      else if (!World.enabled && !shouldDisableWorldInput)
        World.Enable();
      if (InputFieldWithHybridNav.CurrentInput != null || ButtonWithHybridNav.CurrentButton != null)
      {
        InputActionAsset.FindAction("UI/Character").Disable();
        InputActionAsset.FindAction("UI/Skills").Disable();
        InputActionAsset.FindAction("UI/Inventory").Disable();
        InputActionAsset.FindAction("UI/Party").Disable();
        InputActionAsset.FindAction("UI/Journal").Disable();
        InputActionAsset.FindAction("UI/Map").Disable();
      }
      else
      {
        InputActionAsset.FindAction("UI/Character").Enable();
        InputActionAsset.FindAction("UI/Skills").Enable();
        InputActionAsset.FindAction("UI/Inventory").Enable();
        InputActionAsset.FindAction("UI/Party").Enable();
        InputActionAsset.FindAction("UI/Journal").Enable();
        InputActionAsset.FindAction("UI/Map").Enable();
      }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (Controls["Menu"].gameObject.activeInHierarchy)
      {
        if (Controls["Menu"].canvasGroup.interactable)
          CloseAll();
      }
    }
  }
}