using System;
using UnityEngine;

namespace ClaraMundi
{
  public class ContextMenuHandler : MonoBehaviour
  {
    public GameObject LastSelectedObject;
    public ItemUI ContextualItem;
    public Player ContextualPlayer;
    public FormElement ContextualFormElement;
    public ContextMenu PlayerMenu;
    public ContextMenu ItemMenu;
    public ContextMenu EquippedMenu;

    public static ContextMenuHandler Instance;

    private void Awake()
    {
      Instance = this;
    }

    public void SetState(ItemUI item, FormElement formElement)
    {
      ContextualItem = item;
      SetState(formElement);
    }
    public void SetState(Player player, FormElement formElement)
    {
      ContextualPlayer = player;
      SetState(formElement);
    }

    public void SetState(FormElement formElement)
    {
      if (formElement != null)
      {
        ContextualFormElement = formElement;
        Form.PreviouslySelected = formElement;
      }
    }

    public void CloseAll()
    {
      ContextualItem = null;
      ContextualPlayer = null;
      ItemMenu.gameObject.SetActive(false);
      PlayerMenu.gameObject.SetActive(false);
      EquippedMenu.gameObject.SetActive(false);
    }
  }
}