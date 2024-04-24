using System;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
  public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public string NodeId;
    public InventoryUI InventoryUI;
    public EquipmentUI EquipmentUI;
    public OwningEntityHolder owner;
    public ItemTooltipUI Tooltip;
    public event Action EntityChange;
    public event Action<ItemUI> OnDoubleClick;
    public ContextMenu ContextMenu;
    private string _entityId;
    public string StorageId = "inventory";

    ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;
    public ItemInstance ItemInstance;
    public Item Item { get; private set; }

    public bool ShowEquippedStatus = true;
    public GameObject EquippedStatus;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI Quantity;
    public Image Icon;

    public ButtonWithHybridNav Button;
    public int ItemInstanceId;
    private bool hasItem;
    private float doubleClickTimer = 0;

    [HideInInspector] public ItemStorage ItemStorage;

    public bool updateQueued;

    public string EquipmentSlot;



    private ItemStorage GetItemStorage()
    {
      if (_entityId == null) return null;
      if (!ItemManager.Instance.StorageByEntityAndId.ContainsKey(_entityId))
        ItemManager.Instance.StorageByEntityAndId[_entityId] = new();
      if (!ItemManager.Instance.StorageByEntityAndId[_entityId].ContainsKey(StorageId))
        return null;
      return ItemManager.Instance.StorageByEntityAndId[_entityId][StorageId];
    }

    private void Start()
    {
      NodeId = StringUtils.UniqueId();
      if (ItemManager.Instance == null) return;
      ItemManager.ItemChange += OnInstanceUpdate;
      if (_entityId != null)
        OnEntityChange("", _entityId);
    }

    public void SetOwner(OwningEntityHolder _owner)
    {
      if (owner != null)
      {
        owner.EntityChange -= OnEntityChange;
      }
      owner = _owner;
      owner.EntityChange += OnEntityChange;
      if (owner.entity != null)
        OnEntityChange("", owner.entity.entityId.Value);
    }


    private void OnEntityChange(string lastId, string entityId)
    {
      _entityId = entityId;
      Debug.Log("Entity Changed: " + entityId);
      ItemStorage = GetItemStorage();
      if (EquipmentUI != null && EquipmentSlot != null)
      {
        ItemInstanceId = 0;
        if (PlayerManager.Instance.Players.ContainsKey(lastId))
          PlayerManager.Instance.Players[lastId].Equipment.EquippedItems.OnChange -= OnEquipChange;
        if (PlayerManager.Instance.Players.ContainsKey(entityId))
        {
          Player player = PlayerManager.Instance.Players[entityId];
          Debug.Log("Listen to equipment changes!");
          player.Equipment.EquippedItems.OnChange += OnEquipChange;
          if (player.Equipment.EquippedItems.ContainsKey(EquipmentSlot))
            ItemInstanceId = player.Equipment.EquippedItems[EquipmentSlot];
        }
      }
      if (ItemStorage == null) return;
      EntityChange?.Invoke();
      updateQueued = true;
    }

    private void OnEquipChange(SyncDictionaryOperation op, string key, int value, bool asServer)
    {
      Debug.Log("Equipment Change - " + key + " - " + value);
      if (EquipmentSlot == key)
      {
        Debug.Log("Change Item " + key + " - " + value);
        ItemInstanceId = value;
        updateQueued = true;
      }
    }


    private void OnInstanceUpdate(SyncDictionaryOperation op, int key, ItemInstance itemInstance, bool asServer)
    {
      if (ItemInstanceId == key)
      {
        updateQueued = true;
      }
    }

    private void OnDisable()
    {
      if (owner?.entity == null) return;
      if (ItemManager.Instance == null) return;
      if (ContextMenu.ContextualItem == this)
        CloseContextMenu();
      HideTooltip();
    }

    private void OnDestroy()
    {
      if (owner?.entity == null) return;
      if (ContextMenu.ContextualItem == this)
        CloseContextMenu();
      HideTooltip();
      ItemManager.ItemChange -= OnInstanceUpdate;
      if (OnDoubleClick != null)
      {
        foreach (var d in OnDoubleClick.GetInvocationList())
          OnDoubleClick -= (Action<ItemUI>)d;
      }

      if (owner != null)
        owner.EntityChange -= OnEntityChange;
      if (owner?.entity != null)
      {
        string entityId = owner.entity.entityId.Value;
        if (PlayerManager.Instance.Players.ContainsKey(entityId))
        {
          Player player = PlayerManager.Instance.Players[entityId];
          player.Equipment.EquippedItems.OnChange -= OnEquipChange;
        }
      }
    }

    private float checkTimer;

    private void Update()
    {
      if (owner?.entity == null) return;
      if (doubleClickTimer > 0)
        doubleClickTimer -= Time.deltaTime;
      if (doubleClickTimer < 0)
        doubleClickTimer = 0;
      if (updateQueued)
      {
        UpdateItem();
        updateQueued = false;
      }

      if (EventSystem.current.currentSelectedGameObject == Button.gameObject)
        ShowTooltip();
      else
        HideTooltip();
      if (ItemStorage != null) return;
      checkTimer += Time.deltaTime;
      if (!(checkTimer > 1)) return;
      checkTimer = 0;
      ItemStorage = GetItemStorage();
      if (ItemStorage == null) return;
      EntityChange?.Invoke();
    }

    private void UpdateItem()
    {

      ItemManager.Instance.ItemsByInstanceId.TryGetValue(ItemInstanceId, out ItemInstance);
      if (ItemInstance != null)
        Item = ItemRepo.GetItem(ItemInstance.ItemId);
      if (ItemInstance != null)
        ShowItemInstance();
      else
        ShowNoItem();
    }

    public void ShowNoItem()
    {
      if (Tooltip.NodeId == NodeId)
      {
        Tooltip.gameObject.SetActive(false);
        if (Tooltip.EquippedTooltip != null)
          Tooltip.EquippedTooltip.gameObject.SetActive(false);
      }

      ItemInstance = null;
      Item = null;
      Icon.sprite = null;
      Icon.color = new Color(255, 255, 255, 0);
      hasItem = false;
      Icon.enabled = false;
      if (ItemName != null)
      {
        ItemName.text = "No Item";
        ItemName.enabled = false;
      }

      if (Quantity != null)
      {
        Quantity.text = "";
        Quantity.enabled = false;
      }

      if (EquippedStatus != null)
        EquippedStatus.SetActive(false);
      // Destroy(gameObject);
    }

    private void ShowItemInstance()
    {
      if (ItemInstance == null || Item == null)
      {
        ShowNoItem();
        return;
      }
      Icon.sprite = Item.Icon;
      Icon.color = new Color(255, 255, 255, 1);
      Icon.enabled = true;
      hasItem = true;
      if (ItemName != null)
      {
        ItemName.text = Item.Name;
        ItemName.enabled = true;
      }

      if (Quantity != null)
      {
        if (Item.Stackable)
          Quantity.text = ItemInstance.Quantity + "";
        else
          Quantity.text = "";
        Quantity.enabled = true;
      }

      if (Tooltip != null && Tooltip.ItemInstance != null &&
          Tooltip.ItemInstance.ItemInstanceId == ItemInstance.ItemInstanceId)
        Tooltip.SetItemInstance(ItemInstance);
      if (ShowEquippedStatus && EquippedStatus != null)
        EquippedStatus.SetActive(ItemInstance.IsEquipped);
    }

    public void LinkToChat()
    {
      ChatWindowUI.Instance.AddItemLink(ItemInstance);
    }

    public void ShowTooltip()
    {
      if (ItemManager.Instance == null) return;
      if (!hasItem) return;
      if (Tooltip.NodeId == NodeId) return;
      ItemTooltipUtils.ShowTooltip(Tooltip, (RectTransform)transform, ItemInstance.ItemInstanceId);
      Tooltip.NodeId = NodeId;
    }

    public void HideTooltip()
    {
      if (ItemManager.Instance == null) return;
      if (Tooltip != null && Tooltip.NodeId != NodeId) return;
      // Assume the last item assigned to the tooltip was this item
      Tooltip.gameObject.SetActive(false);
      Tooltip.NodeId = null;
      if (Tooltip.EquippedTooltip != null)
        Tooltip.EquippedTooltip.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (!Button.interactable) return;
      if (EventSystem.current.currentSelectedGameObject != Button.gameObject)
        EventSystem.current.SetSelectedGameObject(Button.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (!Button.interactable) return;
      if (EventSystem.current.currentSelectedGameObject == Button.gameObject)
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenContextMenu()
    {
      if (Item == null || ItemInstance == null) return;
      ContextMenu.ContextualItem = this;
      ContextMenu.SetItemActive("Drop", Item.Droppable && ShowEquippedStatus);
      ContextMenu.SetItemActive("Use", Item.Type == ItemType.Consumable && ShowEquippedStatus);
      var isEquipped = ItemInstance.IsEquipped;
      ContextMenu.ChangeLabelOf("Equip", isEquipped ? "Take Off" : "Equip");
      ContextMenu.SetItemActive("Equip", Item.Equippable);
      ContextMenu.SetItemActive("Split", ItemInstance.Quantity > 1 && ShowEquippedStatus);
      ContextMenu.gameObject.SetActive(true);
      ContextMenu.transform.position = transform.position;

      ContextMenu.OnAction += OnAction;
      ContextMenu.OnClose += OnContextClose;
      InventoryUI.Focus.canvasGroup.interactable = false;

      ContextMenu.SelectFirstElement();
    }
    void OnAction(string action)
    {
      switch (action)
      {
        case "Use":
          InventoryUI.UseItem(this);
          break;
        case "Equip":
          InventoryUI.EquipItem(this);
          break;
        case "Unequip":
        case "Take Off":
          InventoryUI.UnequipItem(this);
          break;
        case "Link to Chat":
          InventoryUI.LinkToChat(this);
          break;
        case "Drop":
          InventoryUI.DropItem(this);
          break;
      }
      InventoryUI.Focus.canvasGroup.interactable = true;
      ContextMenu.gameObject.SetActive(false);
      InventoryUI.Focus.Select();
    }
    void OnContextClose()
    {
      ContextMenu.OnAction -= OnAction;
      ContextMenu.OnClose -= OnContextClose;
    }

    public void CloseContextMenu()
    {
      ContextMenu.ContextualItem = null;
      ContextMenu.gameObject.SetActive(false);
      ContextMenu.transform.localPosition = new Vector2(0, 0);
    }
  }
}