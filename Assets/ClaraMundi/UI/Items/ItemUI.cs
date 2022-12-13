using System;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClaraMundi
{
    public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public string NodeId = Guid.NewGuid().ToString();
        public OwningEntityHolder owner;
        public ItemTooltipUI Tooltip;
        public event Action EntityChange;
        public event Action<ItemUI> OnDoubleClick;
        public event Action<ItemUI, PointerEventData> OnContextMenu;
        private string _entityId;
        public string StorageId = "inventory";
        bool isHovering;

        ItemRepo ItemRepo => RepoManager.Instance.ItemRepo;
        public ItemInstance ItemInstance;
        public Item Item { get; private set; }

        public bool ShowEquippedStatus = true;
        public GameObject EquippedStatus;
        public TextMeshProUGUI ItemName;
        public TextMeshProUGUI Quantity;
        public Image Icon;

        public string ItemInstanceId;
        private Image Background;
        private float checkTick;
        private bool hasItem;
        private float doubleClickTimer = 0;

        [HideInInspector] public ItemStorage ItemStorage;

        public bool updateQueued;


        private ItemStorage GetItemStorage()
        {
            if (_entityId == null) return null;
            if (!ItemManager.Instance.StorageByEntityAndId.ContainsKey(_entityId))
                ItemManager.Instance.StorageByEntityAndId[_entityId] = new();
            if (!ItemManager.Instance.StorageByEntityAndId[_entityId].ContainsKey(StorageId))
                return null;
            return ItemManager.Instance.StorageByEntityAndId[_entityId][StorageId];
        }

        private void Awake()
        {
            Background = GetComponent<Image>();
            ItemManager.ItemChange += OnInstanceUpdate;
            if (_entityId != null)
                OnEntityChange(_entityId);
        }

        public void SetOwner(OwningEntityHolder _owner)
        {
            owner = _owner;
            owner.EntityChange += OnOwnerEntityChange;
            OnOwnerEntityChange();
        }

        void OnOwnerEntityChange()
        {
            OnEntityChange(owner.entity ? owner.entity.entityId : null);
        }

        private void OnEntityChange(string entityId)
        {
            _entityId = entityId;
            ItemStorage = GetItemStorage();
            if (ItemStorage == null) return;
            EntityChange?.Invoke();
            Initialize();
        }

        public void Initialize()
        {
            var lastItem = ItemInstanceId;
            ItemInstanceId = ItemInstance?.ItemInstanceId;
            if (ItemInstance != null)
                Item = ItemRepo.GetItem(ItemInstance.ItemId);
            if (lastItem != ItemInstanceId)
                updateQueued = true;
        }

        private void OnInstanceUpdate(SyncDictionaryOperation op, string key, ItemInstance itemInstance, bool asServer)
        {
            if (itemInstance != null && ItemInstanceId == itemInstance.ItemInstanceId)
                updateQueued = true;
        }

        private void OnDestroy()
        {
            // if (Tooltip != null && Tooltip.NodeId == NodeId)
            //     Tooltip.gameObject.SetActive(false);
            OnContextMenu?.Invoke(this, null);
            ItemManager.ItemChange -= OnInstanceUpdate;
            if (OnDoubleClick != null)
            {
                foreach (var d in OnDoubleClick.GetInvocationList())
                    OnDoubleClick -= (Action<ItemUI>)d;
            }

            if (OnContextMenu != null)
            {
                foreach (var d in OnContextMenu.GetInvocationList())
                    OnContextMenu -= (Action<ItemUI, PointerEventData>)d;
            }

            if (owner != null)
                owner.EntityChange -= OnOwnerEntityChange;
        }

        private float checkTimer;

        private void Update()
        {
            if (doubleClickTimer > 0)
                doubleClickTimer -= Time.deltaTime;
            if (doubleClickTimer < 0)
                doubleClickTimer = 0;
            if (updateQueued)
            {
                UpdateItem();
                updateQueued = false;
            }

            if (EquippedStatus != null)
                EquippedStatus.SetActive(ShowEquippedStatus && ItemInstance is { IsEquipped: true });

            if (ItemStorage != null) return;
            checkTimer += Time.deltaTime;
            if (!(checkTimer > 1)) return;
            checkTimer = 0;
            ItemStorage = GetItemStorage();
            if (ItemStorage == null) return;
            EntityChange?.Invoke();
            Initialize();
        }

        private void UpdateItem()
        {
            if (ItemInstanceId == null)
            {
                ShowNoItem();
                return;
            }

            if (ItemManager.Instance.ItemsByInstanceId.TryGetValue(ItemInstanceId, out ItemInstance))
            {
                Item = ItemRepo.GetItem(ItemInstance.ItemId);
                ShowItemInstance();
            }
            else
            {
                ShowNoItem();
            }
        }

        public void ShowNoItem()
        {
            if (Tooltip.NodeId == NodeId)
            {
                Tooltip.gameObject.SetActive(false);
                if (Tooltip.EquippedTooltip != null)
                    Tooltip.EquippedTooltip.gameObject.SetActive(false);
                Background.enabled = false;
                transform.localScale = Vector3.zero;
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
            Destroy(gameObject);
        }

        private void ShowItemInstance()
        {
            if (ItemInstance == null) return;
            Icon.sprite = Item.Icon;
            Icon.color = new Color(255, 255, 255, 1);
            hasItem = true;
            Icon.enabled = true;
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
        
            if (Tooltip != null && Tooltip.ItemInstance != null && Tooltip.ItemInstance.ItemInstanceId == ItemInstance.ItemInstanceId)
                Tooltip.SetItemInstance(ItemInstance);
        }

        public void LinkToChat()
        {
            ChatWindowUI.Instance.AddItemLink(ItemInstance);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            if (hasItem)
            {
                Tooltip.gameObject.SetActive(false);
                if (Tooltip.EquippedTooltip != null)
                    Tooltip.EquippedTooltip.gameObject.SetActive(false);
            }

            Background.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!hasItem) return;
            isHovering = true;
            Background.enabled = true;
            Tooltip.NodeId = NodeId;
            Tooltip.SetItemInstance(ItemInstance);
            var position = transform.position;
            int horizontal = ScreenUtils.GetHorizontalWithMostSpace(position.x);
            int vertical = ScreenUtils.GetVerticalWithMostSpace(position.y);
            var transform1 = Tooltip.transform;
            RectTransform rect = (RectTransform)transform1;
            var size = ActualSize((RectTransform)transform);
            var rect2 = rect.rect;
            var verticalOffset = size.y / 2;
            if (vertical == -1)
            {
                verticalOffset = rect2.height - (size.y / 2);
            }

            transform1.position = new Vector3(
                position.x + (horizontal * (size.x / 2 + (rect2.width / 2) + 8)),
                position.y + verticalOffset,
                0
            );
            if (Tooltip.EquippedTooltip != null)
            {
                var position1 = transform1.position;
                Tooltip.EquippedTooltip.transform.position = new Vector3(
                position1.x + (horizontal * 248),
                    position1.y ,
                0
                );
            }

            Tooltip.gameObject.SetActive(true);
        }


        public Vector2 ActualSize(RectTransform rect)
        {
            Canvas can = GetComponentInParent<Canvas>();
            var v = new Vector3[4];
            rect.GetWorldCorners(v);
            //method one
            //return new Vector2(v[3].x - v[0].x, v[1].y - v[0].y);

            //method two
            return RectTransformUtility.PixelAdjustRect(rect, can).size;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!hasItem) return;
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnContextMenu?.Invoke(this, eventData);
            }
            else
            {
                if (doubleClickTimer == 0)
                    doubleClickTimer = 0.5f;
                else
                {
                    OnDoubleClick?.Invoke(this);
                }
            }
        }
    }
}