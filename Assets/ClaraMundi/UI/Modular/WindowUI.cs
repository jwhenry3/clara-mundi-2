using System;
using System.Collections.Generic;
using JoshH.UI;
using UnityEngine;
using UnityEngine.Events;
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
    private Vector4 BorderRadius = new Vector4(4, 4, 4, 4);
    public Color background = Color.black;
    [HideInInspector]
    public Color lastBackground;
    [Header("Utilities")]
    public WindowUI parent;
    public MoveSibling moveSibling;
    [HideInInspector]
    public Layout layout;
    [HideInInspector]
    public ProceduralImage proceduralImage;
    [HideInInspector]
    public FreeModifier freeModifier;
    public WindowUI CurrentWindow;

    [HideInInspector]
    private UIGradient gradient;

    private float tick;
    private float interval = 0.2f;
    private bool listening;

    public bool blockCancel;
    public event Action CancelPressed;
    public UnityEvent OnCancelled = new();

    public void OnEnable()
    {
      lastBackground = background;
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
        CancelPressed?.Invoke();
        OnCancelled?.Invoke();
        if (blockCancel) return;
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
      }
      if (Application.isPlaying && hideWhenNotInFront && CurrentWindow == null && !moveSibling.IsInFront())
        gameObject.SetActive(false);
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
        gradient = gradient ?? GetComponent<UIGradient>() ?? gameObject.AddComponent<UIGradient>();
        gradient.LinearColor1 = new Color(0, 0.4f, 0.8f, 1);
        gradient.LinearColor2 = new Color(0, 0.2f, 0.4f, 1);
        if (proceduralImage == null)
          proceduralImage = GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
        proceduralImage.ModifierType = typeof(FreeModifier);
        if (freeModifier == null)
          freeModifier = GetComponent<FreeModifier>() ?? gameObject.AddComponent<FreeModifier>();
        proceduralImage.color = background;
        if (layout == null)
          layout = GetComponent<Layout>() ?? gameObject.AddComponent<Layout>();
        if (moveSibling == null)
        {
          moveSibling = GetComponent<MoveSibling>() ?? gameObject.AddComponent<MoveSibling>();
          moveSibling.MovingObject = transform;
        }
        if (freeModifier != null)
          freeModifier.Radius = BorderRadius;
      }
    }

  }
}