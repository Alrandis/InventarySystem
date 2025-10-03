using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private GameObject _selectionHighlight;
    [SerializeField] private Button _dropButton;

    public event Action OnDropClicked; // событие наружу
    public event Action<ItemSlotUI> OnClicked;

    private IItemInstance _currentItem;
    private ItemTooltip _tooltip;
    private bool _isSelected;
    private InventoryUI _inventoryUI;
    private Inventory _inventory;


    // Drag visual
    public Image DraggedIcon { get; private set; }
    private Canvas _canvas;

    public int Index { get; private set; }

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();

        _dropButton.onClick.AddListener(() => {OnDropClicked?.Invoke();});
        _dropButton.gameObject.SetActive(false); // скрыта по умолчанию
    }

    public void Init(int index, InventoryUI inventoryUI, Inventory inventory, ItemTooltip tooltip)
    {
        Index = index;
        _inventoryUI = inventoryUI;
        _inventory = inventory;
        _tooltip = tooltip;
    }

    public void SetItem(IItemInstance item)
    {
        _currentItem = item;
        if (item == null)
        {
            Clear(); // Clear() внутри вызывает Deselect()
            return;
        }

        _itemIcon.sprite = item.ItemData.Icon;
        _itemIcon.enabled = true;

        if (item is IStackable stackable)
        {
            _stackText.text = stackable.CurrentStack.ToString();
            _stackText.enabled = true;
        }
        else
        {
            _stackText.enabled = false;
        }
    }

    public void Clear()
    {
        _currentItem = null;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
        _stackText.text = "";
        _stackText.enabled = false;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectionHighlight) _selectionHighlight.SetActive(_isSelected);
        _dropButton?.gameObject.SetActive(_isSelected);
    }

    public IItemInstance GetItem() => _currentItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            _tooltip.Show(_currentItem.ItemData.Name, _currentItem.ItemData.Description, Input.mousePosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltip.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        if (eventData.clickCount == 1)
            HandleSingleClick();
        else if (eventData.clickCount == 2)
            HandleDoubleClick();
    }

    private void HandleSingleClick()
    {
        // ”правление выделением только через InventoryUI
        if (_inventoryUI != null)
        {
            _inventoryUI.SelectSlot(this);
        } 
    }

    private void HandleDoubleClick()
    {
        if (_currentItem is IUsableItem usable)
        {
            usable.Use();
            if (_inventory != null) _inventory.HandleItemUsed(_currentItem);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        // создаЄм визуал и передаЄм в InventoryUI
        DraggedIcon = new GameObject("DraggedIcon").AddComponent<Image>();
        DraggedIcon.raycastTarget = false;
        DraggedIcon.sprite = _itemIcon.sprite;
        DraggedIcon.transform.SetParent(_canvas.transform, false);
        DraggedIcon.transform.SetAsLastSibling();

        // масштаб/размер
        DraggedIcon.rectTransform.sizeDelta = _itemIcon.rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DraggedIcon != null)
            DraggedIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DraggedIcon != null)
            Destroy(DraggedIcon.gameObject);

        // raycast на UI под курсором
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        ItemSlotUI targetSlot = null;
        foreach (var r in results)
        {
            targetSlot = r.gameObject.GetComponent<ItemSlotUI>();
            if (targetSlot != null) break;
        }

        if (targetSlot != null)
        {
            if (_inventoryUI != null) _inventoryUI.SwapSlots(this, targetSlot);
        }

        DraggedIcon = null;
    }
}
