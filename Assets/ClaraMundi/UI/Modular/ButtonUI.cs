using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
namespace ClaraMundi
{
  [ExecuteInEditMode]
  public class ButtonUI : MonoBehaviour
  {
    public static List<ButtonUI> buttons = new();
    public static Color globalColor;
    [Header("Options")]
    public bool HasText = true;
    public bool HasIcon = true;
    [Header("Text Options")]
    public bool StretchText = true;
    public TextAlignmentOptions TextAlignment = TextAlignmentOptions.Center;
    [Header("Visual Options")]
    [SerializeField]
    public Color background = Color.black;
    [HideInInspector]
    public Color lastBackground;
    public Vector4 BorderRadius = new Vector4(8, 8, 8, 8);
    public Sprite iconSprite;
    [Header("Utilities")]
    public Button button;
    public TextMeshProUGUI text;
    public LayoutElement textElement;
    public ContentSizeFitter textFitter;
    public Image icon;
    protected LayoutElement iconElement;
    public Layout layout;

    public ProceduralImage proceduralImage;
    public FreeModifier freeModifier;

    void LateUpdate()
    {
      SetUp();
    }
    void OnDestroy()
    {
      buttons.Remove(this);
    }
    protected virtual void SetUp()
    {
      if (!Application.isPlaying)
      {
        if (!buttons.Contains(this))
          buttons.Add(this);
        transform.localScale = Vector3.one;
        if (lastBackground != background)
        {
          buttons.ForEach((b) =>
          {
            b.background = background;
            b.lastBackground = background;
          });
          lastBackground = background;
        }
        if (proceduralImage == null)
          proceduralImage = gameObject.GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
        proceduralImage.ModifierType = typeof(FreeModifier);
        if (freeModifier == null)
          freeModifier = gameObject.GetComponent<FreeModifier>() ?? gameObject.AddComponent<FreeModifier>();
        proceduralImage.color = background;
        if (layout == null)
          layout = gameObject.GetComponent<Layout>() ?? gameObject.AddComponent<Layout>();
        if (button == null)
          button = gameObject.GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        if (text == null && HasText)
        {
          text = gameObject.GetComponentsInChildren<TextMeshProUGUI>().Skip(1).FirstOrDefault();
          if (text == GetComponent<TextMeshProUGUI>())
            text = null;
          if (text == null)
          {
            var obj = new GameObject();
            obj.name = "Text";
            obj.transform.parent = transform;
            textElement = obj.AddComponent<LayoutElement>();
            text = obj.AddComponent<TextMeshProUGUI>();
          }
        }
        if (icon == null && HasIcon)
        {
          icon = gameObject.GetComponentsInChildren<Image>().Skip(1).FirstOrDefault();
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
        if (text != null && !HasText && text != GetComponent<TextMeshProUGUI>())
        {
          DestroyImmediate(text.gameObject);
          text = null;
          textElement = null;
        }
        if (text != null && textElement == null)
          textElement = text.GetComponent<LayoutElement>() ?? text.AddComponent<LayoutElement>();
        if (text != null && textFitter == null)
          textFitter = text.GetComponent<ContentSizeFitter>() ?? text.AddComponent<ContentSizeFitter>();
        if (icon != null && iconElement == null)
          iconElement = icon.GetComponent<LayoutElement>() ?? icon.AddComponent<LayoutElement>();
        if (icon != null)
        {
          iconElement.preferredWidth = 16;
          iconElement.preferredHeight = 16;
          iconElement.flexibleWidth = 0;
          iconElement.flexibleHeight = 0;
        }
        if (icon != null)
        {
          icon.sprite = iconSprite;
        }
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
          text.fontSize = 20;
          text.alignment = TextAlignment;
        }
        if (!freeModifier.Radius.Equals(BorderRadius))
          freeModifier.Radius = BorderRadius + Vector4.zero;
        if (layout.align != TextAnchor.MiddleLeft)
          layout.align = TextAnchor.MiddleLeft;
      }
    }
  }
}