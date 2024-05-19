using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
  [ExecuteInEditMode]

  public class WindowUI : MonoBehaviour
  {
    public static Dictionary<string, WindowUI> windows = new();
    [Header("Options")]
    public bool hideWhenNotInFront;
    [Header("Input")]
    public string TriggerAction;

    [Header("Visual Options")]
    public Vector4 BorderRadius = new Vector4(8, 8, 8, 8);
    public Color background = Color.black;
    [HideInInspector]
    public Color lastBackground;
    [Header("Utilities")]
    public WindowUI parent;
    public MoveSibling moveSibling;
    public Layout layout;
    public ProceduralImage proceduralImage;
    public FreeModifier freeModifier;

    public ButtonUI CurrentButton;
    public WindowUI CurrentWindow;

    private float tick;
    private float interval = 0.2f;
    private bool listening;

    public void OnEnable()
    {
      if (parent == null)
        parent = GetComponentInParent<WindowUI>();
      if (parent == this)
        parent = null;
      SetUp();
      if (Application.isPlaying)
      {
        moveSibling.SentToFront += OnSentToFront;
        moveSibling.SentToBack += OnSentToBack;
        if (moveSibling.IsInFront())
          OnSentToFront();
      }
    }

    void OnCancel(InputAction.CallbackContext context)
    {
      if (moveSibling.IsInFront())
      {
        if (parent != null && parent.CurrentWindow == this)
        {
          parent.CurrentWindow = null;
          parent.moveSibling.ToFront();
        }
        moveSibling.ToBack();
      }
    }

    void OnDisable()
    {
      if (Application.isPlaying)
      {
        moveSibling.SentToFront -= OnSentToFront;
        moveSibling.SentToBack -= OnSentToBack;
        if (listening && InputManager.Instance != null)
        {
          InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
          listening = false;
          if (parent != null && parent.CurrentWindow == this)
            parent.CurrentWindow = null;
        }
      }
    }

    void OnSentToFront()
    {
      if (CurrentButton != null && CurrentWindow == null)
      {
        EventSystem.current.SetSelectedGameObject(CurrentButton.gameObject);
      }
      if (parent != null && parent.CurrentWindow != this)
      {
        parent.CurrentWindow = this;
        parent.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        parent.gameObject.SetActive(true);
      }
      if (!listening && InputManager.Instance != null)
      {
        InputManager.Instance.UI.FindAction("Cancel").performed += OnCancel;
        listening = true;
      }
    }
    void OnSentToBack()
    {
      if (listening && InputManager.Instance != null)
      {
        InputManager.Instance.UI.FindAction("Cancel").performed -= OnCancel;
        if (parent != null && parent.CurrentWindow == this)
          parent.CurrentWindow = null;
        listening = false;
      }
    }

    void Update()
    {
      tick += Time.deltaTime;
      if (tick > interval)
      {
        tick = 0;
        SetUp();
        if (Application.isPlaying && hideWhenNotInFront)
          gameObject.SetActive(CurrentWindow != null || moveSibling.IsInFront());
      }
    }

    void OnDestroy()
    {
      windows.Remove(gameObject.name);
    }

    public void SetUp()
    {
      if (!Application.isPlaying)
      {
        if (!windows.ContainsKey(gameObject.name))
          windows.Add(gameObject.name, this);
        transform.localScale = Vector3.one;
        if (lastBackground != background)
        {
          foreach (var kvp in windows)
          {
            var w = kvp.Value;
            w.background = background;
            w.lastBackground = background;
          }
          lastBackground = background;
        }
        if (proceduralImage == null)
          proceduralImage = GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
        proceduralImage.ModifierType = typeof(FreeModifier);
        if (freeModifier == null)
          freeModifier = GetComponent<FreeModifier>() ?? gameObject.AddComponent<FreeModifier>();
        proceduralImage.color = background;
        if (layout == null)
          layout = GetComponent<Layout>() ?? gameObject.AddComponent<Layout>();
        if (moveSibling == null)
          moveSibling = GetComponent<MoveSibling>() ?? gameObject.AddComponent<MoveSibling>();
        moveSibling.MovingObject = transform;
        if (freeModifier != null)
          freeModifier.Radius = BorderRadius;
      }
    }

  }
}