using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
namespace ClaraMundi
{
  [ExecuteInEditMode]
  public class Panel : MonoBehaviour
  {
    public Color background = Color.cyan;

    public ProceduralImage proceduralImage;
    public FreeModifier freeModifier;



    void LateUpdate()
    {
      SetUp();
    }

    protected virtual void SetUp()
    {

      if (!Application.isPlaying)
      {
        if (proceduralImage == null)
          proceduralImage = gameObject.GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
        proceduralImage.ModifierType = typeof(FreeModifier);
        if (freeModifier == null)
          freeModifier = gameObject.GetComponent<FreeModifier>() ?? gameObject.AddComponent<FreeModifier>();
        proceduralImage.color = background;
      }
    }
  }
}