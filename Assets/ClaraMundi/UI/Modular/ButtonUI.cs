using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
namespace ClaraMundi
{
  [ExecuteInEditMode]
  public class ButtonUI : MonoBehaviour
  {
    public WindowUI window;
    public WindowUI targetWindow;
    public static List<ButtonUI> buttons = new();
    public static Color globalColor;
    [Header("Options")]
    public bool HasText = true;
    public bool UseNameAsText = true;
    public bool HasIcon = true;
    public bool HasInitialFocus = false;


    [Header("Text Options")]
    public bool StretchText = true;
    public TextAlignmentOptions TextAlignment = TextAlignmentOptions.Center;
    [Header("Visual Options")]
    [SerializeField]
    public Color background = Color.black;
    public bool showIconOnSelect;
    [HideInInspector]
    public Color lastBackground;
    public Vector4 BorderRadius = new Vector4(8, 8, 8, 8);
    public Sprite iconSprite;
    [Header("Utilities")]
    public Button button;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI text;
    public LayoutElement textElement;
    public ContentSizeFitter textFitter;
    public Image icon;
    protected LayoutElement iconElement;
    public Layout layout;

    public ProceduralImage proceduralImage;
    public FreeModifier freeModifier;

    private float tick;
    private float interval = 0.2f;

    void OnEnable()
    {
      if (Application.isPlaying)
        if (button != null && targetWindow != null)
        {
          button.onClick.AddListener(targetWindow.moveSibling.ToFront);
        }
      if (window != null && window.CurrentButton == null && HasInitialFocus)
        window.CurrentButton = this;
    }

    void OnDisable()
    {
      if (Application.isPlaying)
        if (button != null && targetWindow != null)
        {
          button.onClick.RemoveListener(targetWindow.moveSibling.ToFront);
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
    }
    void OnDestroy()
    {
      buttons.Remove(this);
    }
    public virtual void SetUp()
    {
      if (window == null)
        window = GetComponentInParent<WindowUI>();
      if (window != null && window.CurrentButton == null && HasInitialFocus)
        window.CurrentButton = this;
      if (!Application.isPlaying)
      {
        if (!buttons.Contains(this))
          buttons.Add(this);
        transform.localScale = Vector3.one;

        if (canvasGroup == null)
          canvasGroup = GetComponentInParent<CanvasGroup>();
        if (layout == null)
          layout = GetComponent<Layout>() ?? gameObject.AddComponent<Layout>();
        if (button == null)
          button = GetComponent<Button>() ?? gameObject.AddComponent<Button>();

        StartCoroutine(PrepareText());
        StartCoroutine(PrepareGraphics());
      }
      if (Application.isPlaying && EventSystem.current != null)
      {
        if (showIconOnSelect && icon != null)
          icon.enabled = EventSystem.current.currentSelectedGameObject == gameObject;
        if (window != null && window.CurrentButton != this && EventSystem.current.currentSelectedGameObject == gameObject)
        {
          window.CurrentButton = this;
        }
      }
    }
    IEnumerator PrepareText()
    {
      yield return new WaitForSeconds(0.01f);
      if (text == null && HasText)
      {
        text = GetComponentsInChildren<TextMeshProUGUI>().Skip(1).FirstOrDefault();
        if (text == GetComponent<TextMeshProUGUI>())
          text = null;
        if (text == null)
        {
          var obj = new GameObject();
          obj.name = "Text";
          obj.transform.parent = transform;
          textElement = obj.AddComponent<LayoutElement>();
          text = obj.AddComponent<TextMeshProUGUI>();
          text.text = "Text";
        }
      }
      if (text != null && !HasText && text != GetComponent<TextMeshProUGUI>())
      {
        DestroyImmediate(text.gameObject);
        text = null;
        textElement = null;
      }
      if (text != null && textElement == null)
        textElement = text.GetComponent<LayoutElement>() ?? text.gameObject.AddComponent<LayoutElement>();
      if (text != null && textFitter == null)
        textFitter = text.GetComponent<ContentSizeFitter>() ?? text.gameObject.AddComponent<ContentSizeFitter>();

      if (textFitter != null)
      {
        textFitter.horizontalFit = StretchText ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
        textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
      }
      if (textElement != null)
      {
        textElement.flexibleWidth = StretchText ? 1 : 0;
      }
      if (text != null)
      {
        text.transform.localScale = Vector3.one;
        text.fontSize = 16;
        text.alignment = TextAlignment;
        if (UseNameAsText)
          text.text = gameObject.name;
      }
    }
    IEnumerator PrepareGraphics()
    {
      if (lastBackground != background)
      {
        buttons.ForEach((b) =>
        {
          b.background = background;
          b.lastBackground = background;
        });
        lastBackground = background;
      }
      yield return new WaitForSeconds(0.01f);
      if (proceduralImage == null)
        proceduralImage = GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
      yield return new WaitForSeconds(0.01f);
      if (freeModifier == null)
        freeModifier = GetComponent<FreeModifier>();
      proceduralImage.color = background;
      if (icon == null && HasIcon)
      {
        icon = GetComponentsInChildren<Image>().Skip(1).FirstOrDefault();
        if (icon == GetComponent<Image>())
          icon = null;
        if (icon == null)
        {
          var obj = new GameObject();
          obj.name = "Icon";
          obj.transform.parent = transform;
          obj.transform.SetAsFirstSibling();
          iconElement = obj.AddComponent<LayoutElement>();
          textFitter = obj.AddComponent<ContentSizeFitter>();
          icon = obj.AddComponent<Image>();
        }
      }
      if (icon != null && !HasIcon && icon != GetComponent<Image>())
      {
        DestroyImmediate(icon.gameObject);
        icon = null;
        iconElement = null;
      }
      if (icon != null && iconElement == null)
        iconElement = icon.GetComponent<LayoutElement>() ?? icon.gameObject.AddComponent<LayoutElement>();
      if (iconElement != null)
      {
        iconElement.preferredWidth = 24;
        iconElement.preferredHeight = 24;
        iconElement.flexibleWidth = 0;
        iconElement.flexibleHeight = 0;
      }
      if (icon != null)
      {
        icon.transform.localScale = Vector3.one;
        if (icon.sprite != iconSprite)
          icon.sprite = iconSprite;
      }
      if (freeModifier != null && !freeModifier.Radius.Equals(BorderRadius))
        freeModifier.Radius = BorderRadius + Vector4.zero;
      if (layout.align != TextAnchor.MiddleLeft)
        layout.align = TextAnchor.MiddleLeft;
    }
  }
}