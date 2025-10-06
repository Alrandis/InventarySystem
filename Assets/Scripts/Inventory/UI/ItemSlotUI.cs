using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlotUI : SlotUI, IPointerClickHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
   
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private Button _dropButton;

    private ItemTooltip _tooltip;
    private ItemTooltipPositioner _tooltipPositioner;

    private Inventory _inventory;

    public event Action OnDropClicked; // событие наружу
    public event Action<ItemSlotUI> OnClicked;
    public int Index { get; private set; }

    protected override void Awake()
    {
        base.Awake();

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

    public override void Clear()
    {
        base.Clear();
        _stackText.text = "";
        _stackText.enabled = false;
    }

    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
        if(_dropButton != null)
            _dropButton.gameObject.SetActive(_isSelected);
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

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedIcon != null)
            Destroy(_draggedIcon.gameObject);

        // raycast на UI под курсором
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            var equipSlot = r.gameObject.GetComponent<EquipSlotUI>();
            if (equipSlot != null)
            {
                if (equipSlot.CanEquip(_currentItem))
                {
                    // Перемещаем предмет в слот экипировки
                    equipSlot.OnDrop(eventData);
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
