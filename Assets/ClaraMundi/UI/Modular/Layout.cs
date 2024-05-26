using UnityEngine;
using UnityEngine.UI;
namespace ClaraMundi
{
  public enum LayoutType
  {
    Vertical,
    Horizontal
  }

  [ExecuteInEditMode]
  public class Layout : MonoBehaviour
  {

    public LayoutType LayoutType;
    public int Padding = 8;
    public int Spacing = 8;

    [HideInInspector]
    public LayoutElement element;
    [HideInInspector]
    public VerticalLayoutGroup verticalLayoutGroup;
    [HideInInspector]
    public HorizontalLayoutGroup horizontalLayoutGroup;
    [HideInInspector]
    public ContentSizeFitter contentSizeFitter;
    public bool stretchHorizontal;
    public bool stretchVertical;
    public bool fitHorizontal = true;
    public bool fitVertical = true;
    public TextAnchor align;
    void LateUpdate()
    {
      SetUp();
    }
    protected virtual void SetUp()
    {
      if (!Application.isPlaying)
      {
        if (element == null)
          element = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
        if (contentSizeFitter == null)
          contentSizeFitter = gameObject.GetComponent<ContentSizeFitter>() ?? gameObject.AddComponent<ContentSizeFitter>();
        if (LayoutType == LayoutType.Vertical && horizontalLayoutGroup != null)
        {
          DestroyImmediate(horizontalLayoutGroup);
          horizontalLayoutGroup = null;
        }
        if (LayoutType == LayoutType.Horizontal && verticalLayoutGroup != null)
        {
          DestroyImmediate(verticalLayoutGroup);
          verticalLayoutGroup = null;
        }
        if (LayoutType == LayoutType.Vertical && verticalLayoutGroup == null)
          verticalLayoutGroup = gameObject.GetComponent<VerticalLayoutGroup>() ?? gameObject.AddComponent<VerticalLayoutGroup>();
        if (LayoutType == LayoutType.Horizontal && horizontalLayoutGroup == null)
          horizontalLayoutGroup = gameObject.GetComponent<HorizontalLayoutGroup>() ?? gameObject.AddComponent<HorizontalLayoutGroup>();
        if (verticalLayoutGroup != null)
          PrepLayout(verticalLayoutGroup);
        if (horizontalLayoutGroup != null)
          PrepLayout(horizontalLayoutGroup);
      }
    }
    void PrepLayout(HorizontalOrVerticalLayoutGroup layout)
    {
      layout.padding.left = Padding;
      layout.padding.right = Padding;
      layout.padding.top = Padding;
      layout.padding.bottom = Padding;
      layout.spacing = Spacing;
      layout.childControlWidth = true;
      layout.childControlHeight = true;
      layout.childForceExpandWidth = stretchHorizontal;
      layout.childForceExpandHeight = stretchVertical;
      layout.childAlignment = align;
      contentSizeFitter.horizontalFit = fitHorizontal ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
      contentSizeFitter.verticalFit = fitVertical ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
    }
  }
}