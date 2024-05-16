using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace ClaraMundi
{
  [ExecuteInEditMode]

  public class GameWindow : MonoBehaviour
  {
    public static List<GameWindow> windows = new();
    [Header("Visual Options")]
    public Vector4 BorderRadius = new Vector4(8, 8, 8, 8);
    public Color background = Color.black;
    [HideInInspector]
    public Color lastBackground;
    [Header("Utilities")]
    public GameWindow parent;
    public CanvasGroup canvasGroup;
    public MoveSibling moveSibling;
    public Layout layout;
    public ProceduralImage proceduralImage;
    public FreeModifier freeModifier;


    public void OnEnable()
    {
      if (parent == null)
        parent = GetComponentInParent<GameWindow>();
      if (parent == this)
        parent = null;
    }

    void LateUpdate()
    {
      SetUp();
    }

    void OnDestroy()
    {
      windows.Remove(this);
    }

    void SetUp()
    {
      if (!Application.isPlaying)
      {
        if (!windows.Contains(this))
          windows.Add(this);
        transform.localScale = Vector3.one;
        if (lastBackground != background)
        {
          windows.ForEach((w) =>
          {
            w.background = background;
            w.lastBackground = background;
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
        if (canvasGroup == null)
          canvasGroup = gameObject.GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        if (moveSibling == null)
          moveSibling = gameObject.GetComponent<MoveSibling>() ?? gameObject.AddComponent<MoveSibling>();
        moveSibling.MovingObject = transform;
      }
    }

  }
}