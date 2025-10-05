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
    private ItemTooltipPositioner _tooltipPositioner;
    private bool _isSelected;
    private InventoryUI _inventoryUI;
    private Inventory _inventory;


    // Drag visual
    public Image DraggedIcon { get; private set; }
    private Canvas _canvas;

    public int Index { get; private set; }
    public IItemInstance CurrentItem => _currentItem;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();

        _dropButton.onClick.AddListener(() => {OnDropClicked?.Invoke();});
        _dropButton.gameObject.SetActive(false); // скрыта по умолчанию
    }

    public void Init(int index, InventoryUI inventoryUI, Inventory inventory, ItemTooltip tooltip, ItemTooltipPositioner tooltipPositioner)
    {
        Index = index;
        _inventoryUI = inventoryUI;
        _inventory = inventory;
        _tooltip = tooltip;
        _tooltipPositioner = tooltipPositioner;
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
            _tooltipPositioner.UpdatePosition(Input.mousePosition);
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
        // Управление выделением только через InventoryUI
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

        // создаём визуал и передаём в InventoryUI
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

        bool handled = false;

        foreach (var r in results)
        {
            var equipSlot = r.gameObject.GetComponent<EquipSlotUI>();
            if (equipSlot != null)
            {
                if (equipSlot.CanEquip(_currentItem))
                {
                    // Перемещаем предмет в слот экипировки
                    equipSlot.OnDrop(eventData);
                    handled = true;
                    break;
                }
            }
        }

        DraggedIcon = null;

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


    // Убирает предмет из модели Inventory (если есть) и возвращает этот IItemInstance.
    // Если слот был только UI-only (нет ссылки на Inventory), то снимает визуал и возвращает локальный _currentItem.
    public IItemInstance ExtractItemFromInventory()
    {
        // Если есть модель — работаем с ней (модель — источник правды)
        if (_inventory != null)
        {
            IItemInstance itemInModel = null;
            if (Index >= 0 && Index < _inventory.Items.Count) // безопасная проверка
                itemInModel = _inventory.Items[Index];

            if (itemInModel != null)
            {
                // Устанавливаем в модели null — это вызовет OnItemChanged и обновит UI через InventoryUI
                _inventory.SetItemAt(Index, null);
                return itemInModel;
            }

            return null;
        }

        // fallback: чисто UI-слот (без модели)
        var temp = _currentItem;
        Clear();
        return temp;
    }

    // Помещает newItem в модель в этот слот и возвращает старый предмет (или null).
    // Используй это, когда нужно "положить" предмет из экипировки в конкретный слот инвентаря.
    public IItemInstance ReplaceItemInInventory(IItemInstance newItem)
    {
        if (_inventory != null)
        {
            IItemInstance old = null;
            if (Index >= 0 && Index < _inventory.Items.Count)
                old = _inventory.Items[Index];

            _inventory.SetItemAt(Index, newItem); // обновит модель и вызовет OnItemChanged -> обновит UI
            return old;
        }

        // fallback: UI-only
        var oldLocal = _currentItem;
        SetItem(newItem); // обновляем визуал
        return oldLocal;
    }

    // Удобный метод-утилита: проверяет, пуст ли слот в модели
    public bool IsEmptyInModel()
    {
        if (_inventory != null)
        {
            if (Index >= 0 && Index < _inventory.Items.Count)
                return _inventory.Items[Index] == null;
            return true;
        }
        return _currentItem == null;
    }

}
