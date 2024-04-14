using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClaraMundi
{
  public class TriggerByAction : MonoBehaviour
  {
    public string ActionName;
    public InputAction InputAction;

    public GameObject ObjectToTrigger;

    public Tabs Tabs;

    void OnEnable()
    {
      if (ActionName == null) return;
      InputAction = InputManager.Instance.UI.FindAction(ActionName);
      if (InputAction == null) return;
      InputAction.performed += OnPerform;
    }
    void OnDisable()
    {
      if (InputAction == null) return;
      InputAction.performed -= OnPerform;
    }

    void OnPerform(InputAction.CallbackContext context)
    {
      ObjectToTrigger.SetActive(!ObjectToTrigger.activeSelf);
      if (!ObjectToTrigger.activeSelf)
      {
        if (Tabs != null)
          Tabs.ChangeTab("");
      }
    }
  }
}