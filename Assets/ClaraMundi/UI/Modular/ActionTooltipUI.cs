using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ActionTooltipUI : MonoBehaviour
  {
    public EntityAction action;
    public Image Icon;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public void Update()
    {
      if (action != null)
      {
        Icon.sprite = action.Sprite;
        Name.text = action.Name;
        Description.text = action.Description;
      }
    }
  }
}