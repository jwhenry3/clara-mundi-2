using Nova;
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

    public LayoutElement element;
    public VerticalLayoutGroup verticalLayoutGroup;
    public HorizontalLayoutGroup horizontalLayoutGroup;
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
      layout.padding.left = 8;
      layout.padding.right = 8;
      layout.padding.top = 8;
      layout.padding.bottom = 8;
      layout.spacing = 8;
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