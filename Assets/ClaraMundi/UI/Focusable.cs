using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace ClaraMundi
{
  public class Focusable : MonoBehaviour
  {
    private Button button;
    public Action OnClick;
    public Color InactiveColor;
    public Color ActiveColor;
    public Image Image;

    public bool IsActivated;
    private bool lastActivated;



    void Start()
    {
      button = GetComponent<Button>();
      if (Image == null)
        Image = GetComponent<Image>();
      button?.onClick.AddListener(OnActivate);
    }

    public void OnActivate()
    {
      OnClick?.Invoke();
    }

    void Update()
    {
      if (Image == null) return;
      if (lastActivated != IsActivated)
      {
        if (IsActivated)
          Image.color = ActiveColor;
        else
          Image.color = InactiveColor;
        lastActivated = IsActivated;
      }
    }


  }
}