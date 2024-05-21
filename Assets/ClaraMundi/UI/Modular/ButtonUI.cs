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
  public class ButtonUI : MonoBehaviour, ISelectHandler
  {
    public WindowUI window;
    public WindowUI targetWindow;
    public static List<ButtonUI> buttons = new();
    public static Color globalColor;
    [Header("Options")]
    public bool HasText = true;
    public bool UseNameAsText = true;
    public bool HasIcon = true;
    public float iconWidth = 24;
    public float iconHeight = 24;
    public bool AutoFocus = false;


    [Header("Text Options")]
    public bool StretchText = true;
    public bool StretchTextHorizontal = false;
    public TextAlignmentOptions TextAlignment = TextAlignmentOptions.Center;
    [Header("Visual Options")]
    [SerializeField]
    private Color background = new Color(0, 0, 0, 0.20f);
    public bool showIconOnSelect;
    private Vector4 BorderRadius = new Vector4(4, 4, 4, 4);
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


    private ScrollRect scroller;
    private GameObject lastSelected;

    void OnEnable()
    {
      scroller = scroller ?? GetComponentInParent<ScrollRect>();
      if (Application.isPlaying)
        if (button != null && targetWindow != null)
        {
          button.onClick.AddListener(targetWindow.moveSibling.ToFront);
        }
      if (window != null && window.CurrentButton == null && window.CurrentInput == null && AutoFocus)
        window.CurrentButton = this;
    }

    public void SnapTo(RectTransform child)
    {
      if (scroller == null) return;
      float padding = 16;
      var scrollRect = scroller;
      float viewportHeight = scrollRect.viewport.rect.height;
      Vector2 scrollPosition = scrollRect.content.anchoredPosition;

      float elementTop = child.anchoredPosition.y;
      float elementBottom = elementTop - child.rect.height;


      float visibleContentTop = -scrollPosition.y - padding;
      float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

      float scrollDelta =
          elementTop > visibleContentTop ? visibleContentTop - elementTop :
          elementBottom < visibleContentBottom ? visibleContentBottom - elementBottom :
          0f;

      scrollPosition.y += scrollDelta;
      scrollRect.content.anchoredPosition = scrollPosition;
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

    void LateUpdate()
    {
      if (lastSelected != gameObject && EventSystem.current.currentSelectedGameObject == gameObject)
        SnapTo(transform as RectTransform);
      lastSelected = EventSystem.current.currentSelectedGameObject;
    }
    void OnDestroy()
    {
      buttons.Remove(this);
    }
    public virtual void SetUp()
    {
      if (window == null)
        window = GetComponentInParent<WindowUI>();
      if (window != null && window.CurrentButton == null && AutoFocus)
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
      }

      if (text != null && text.gameObject.activeInHierarchy != HasText)
      {
        text.gameObject.SetActive(HasText);
        if (UseNameAsText)
          text.text = gameObject.name;
      }
      if (icon != null && icon.gameObject.activeInHierarchy != HasIcon)
      {
        icon.gameObject.SetActive(HasIcon);
        icon.sprite = iconSprite;
      }
    }
    IEnumerator PrepareText()
    {
      yield return new WaitForSeconds(0.01f);
      if (text == null)
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
      if (text != null && textElement == null)
        textElement = text.GetComponent<LayoutElement>() ?? text.gameObject.AddComponent<LayoutElement>();
      if (text != null && textFitter == null)
        textFitter = text.GetComponent<ContentSizeFitter>() ?? text.gameObject.AddComponent<ContentSizeFitter>();

      if (textFitter != null)
      {
        textFitter.horizontalFit = StretchTextHorizontal ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
        textFitter.verticalFit = StretchText ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
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
      yield return new WaitForSeconds(0.01f);
      if (proceduralImage == null)
        proceduralImage = GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
      yield return new WaitForSeconds(0.01f);
      if (freeModifier == null)
        freeModifier = GetComponent<FreeModifier>();
      proceduralImage.color = background;
      if (icon == null)
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
      if (icon != null && iconElement == null)
        iconElement = icon.GetComponent<LayoutElement>() ?? icon.gameObject.AddComponent<LayoutElement>();
      if (iconElement != null)
      {
        iconElement.preferredWidth = iconWidth;
        iconElement.preferredHeight = iconHeight;
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
    }

    public void OnSelect(BaseEventData eventData)
    {
      if (Application.isPlaying && window != null)
      {
        window.CurrentInput = null;
        window.CurrentButton = this;
      }
    }
  }
}