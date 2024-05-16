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

  public class GameWindow : MonoBehaviour
  {
    public GameWindow parent;
    public LayoutType LayoutType;
    protected CanvasGroup canvasGroup;
    protected VerticalLayoutGroup verticalLayoutGroup;
    protected HorizontalLayoutGroup horizontalLayoutGroup;
    protected ContentSizeFitter contentSizeFitter;
    protected MoveSibling moveSibling;

    protected Panel panel;


    public void OnEnable()
    {
      if (parent == null)
        parent = GetComponentInParent<GameWindow>();
      if (parent == this)
        parent = null;
      SetUp();
    }

    void LateUpdate()
    {
      SetUp();
    }

    void SetUp()
    {
      if (!Application.isPlaying)
      {
        if (panel == null)
          panel = gameObject.GetComponent<Panel>() ?? gameObject.AddComponent<Panel>();
        if (canvasGroup == null)
          canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (contentSizeFitter == null)
          contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        if (moveSibling == null)
          moveSibling = gameObject.AddComponent<MoveSibling>();
        moveSibling.MovingObject = transform;
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
      if (layout is VerticalLayoutGroup)
      {
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
      }
      else
      {
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
      }
      contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
      contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

  }
}