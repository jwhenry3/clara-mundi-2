using UnityEngine;
using UnityEngine.UI.ProceduralImage;
namespace ClaraMundi
{
  [ExecuteInEditMode]
  public class Panel : MonoBehaviour
  {
    public Color background;
    protected ProceduralImage proceduralImage;
    protected FreeModifier freeModifier;


    void LateUpdate()
    {
      SetUp();
    }

    void SetUp()
    {

      if (!Application.isPlaying)
      {
        if (proceduralImage == null)
          proceduralImage = gameObject.GetComponent<ProceduralImage>() ?? gameObject.AddComponent<ProceduralImage>();
        proceduralImage.ModifierType = typeof(FreeModifier);
        proceduralImage.color = background;
      }
    }
  }
}