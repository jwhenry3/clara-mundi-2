using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClaraMundi
{
  [ExecuteInEditMode]
  public class InventoryItemUI : MonoBehaviour
  {
    public Color EquippedColor;
    public Color EquippableColor;
    public Color UsableColor;
    public ButtonUI button;
    public ItemInstance instance;
    public Item item;
    public TextMeshProUGUI quantity;
    public LayoutElement quantityElement;
    public ContentSizeFitter quantityFitter;

    private UnityEvent buttonClick;

    public event Action<InventoryItemUI> OnChosen;

    private float tick;
    private float interval = 0.2f;

    public ActionBarAction action = new();

    void OnEnable()
    {
      action = new();
      action.Action = RepoManager.Instance.ActionRepo.Get("/item");
      action.ActionArgs = new();
      action.ActionArgs["item"] = "";
      if (button == null)
        button = GetComponent<ButtonUI>();
      if (button != null)
      {
        button.canvasGroupWatcher = button.canvasGroupWatcher ?? button.GetComponentInParent<CanvasGroupWatcher>(true);
        if (button.canvasGroupWatcher.AutoFocusButton == null)
          button.AutoFocus = true;
        button.HasIcon = true;
        button.HasText = true;
        button.UseNameAsText = false;
      }
      if (item != null && button != null)
      {
        gameObject.name = item.Name;
      }
      if (Application.isPlaying)
      {
        if (button != null && button.button != null)
          (buttonClick = button.button.onClick).AddListener(OnClick);
      }
    }
    void OnDisable()
    {
      if (buttonClick != null)
      {
        buttonClick.RemoveListener(OnClick);
        buttonClick = null;
      }
    }
    void OnClick()
    {
      if (item != null && instance != null)
      {
        OnChosen?.Invoke(this);
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

    public virtual void SetUp()
    {
      if (button == null)
        button = GetComponent<ButtonUI>();
      if (button != null)
      {
        button.TextAlignment = TextAlignmentOptions.Left;
        if (button.layout != null)
        {
          button.layout.fitHorizontal = false;
          button.layout.fitVertical = false;
        }
      }
      StartCoroutine(PrepareText());
      if (button != null)
      {
        if (button.layout != null)
          button.layout.LayoutType = LayoutType.Horizontal;
        if (item != null)
        {
          action.ItemId = item.ItemId;
          action.ActionArgs["item"] = item.ItemId;
          if (button.iconSprite != item.Icon)
            button.iconSprite = item.Icon;
          if (Application.isPlaying)
            button.icon.sprite = item.Icon;
          if (button.text != null)
            button.text.text = item.Name;
          if (quantity != null)
            if (instance != null)
              quantity.text = instance.Quantity > 1 ? instance.Quantity + "" : "";
            else
              quantity.text = "99";
          if (item != null && item.Equippable)
            button.text.color = instance.IsEquipped ? EquippedColor : EquippableColor;
          if (item != null && item.Usable)
            button.text.color = UsableColor;
        }
        else
        {
          if (button.iconSprite != null)
            button.iconSprite = null;
          if (Application.isPlaying)
            button.icon.sprite = null;
          if (button.text != null)
            button.text.text = "N/A";
          if (quantity != null)
            quantity.text = "";
        }
      }
    }

    IEnumerator PrepareText()
    {
      yield return new WaitForSeconds(0.01f);

      if (quantity == null)
      {
        var obj = new GameObject();
        obj.name = "Quantity";
        obj.transform.parent = transform;
        quantityElement = obj.AddComponent<LayoutElement>();
        quantity = obj.AddComponent<TextMeshProUGUI>();
        quantity.text = "99";
      }
      else
      {
        quantity.transform.localScale = Vector3.one;
      }
      quantity.fontSize = 16;
      if (quantity != null && quantityElement == null)
        quantityElement = quantity.GetComponent<LayoutElement>() ?? quantity.gameObject.AddComponent<LayoutElement>();
      if (quantity != null && quantityFitter == null)
        quantityFitter = quantity.GetComponent<ContentSizeFitter>() ?? quantity.gameObject.AddComponent<ContentSizeFitter>();
      if (quantityFitter != null)
      {
        quantityFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        quantityFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
      }
    }
  }
}