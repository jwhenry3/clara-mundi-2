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


    void Start()
    {
      button = GetComponent<Button>();
    }

    void OnEnable()
    {
      button?.onClick.AddListener(OnActivate);
    }

    void OnDisable()
    {
      button?.onClick.RemoveListener(OnActivate);
    }

    public void OnActivate()
    {
      OnClick?.Invoke();
    }

    void Update()
    {
      if (IsActivated)
        Image.color = ActiveColor;
      else
        Image.color = InactiveColor;
    }


  }
}