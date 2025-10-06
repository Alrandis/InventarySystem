using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

[System.Serializable]
public enum EquipSlotType
{
    Helmet,
    Torso,
    Bracer,
    Boots,
    Shield,
    WeaponLeft,
    WeaponRight
}

public class EquipSlotUI : SlotUI, IPointerClickHandler, IDropHandler, IEndDragHandler
{
    // —уществующие пол€
    [SerializeField] private EquipSlotType _slotType;

    private EquipManager _equipManager;
    private bool _isPlaceholder = false;
    public bool IsPlaceholder => _isPlaceholder;
    public EquipSlotType SlotType => _slotType;
    public bool IsDuplicate { get; private set; } = false;

    public event Action OnItemChanged;


    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(EquipManager equipManager)
    {
        _equipManager = equipManager;
    }

    public bool CanEquip(IItemInstance item)
    {
        if (item == null) return false;

        switch (_slotType)
        {
            case EquipSlotType.Helmet:
                return item is ArmorInstance armor && armor.ArmorItem.ArmorType == ArmorType.Helmet;
            case EquipSlotType.Torso:
                return item is ArmorInstance torso && torso.ArmorItem.ArmorType == ArmorType.Torso;
            case EquipSlotType.Bracer:
                return item is ArmorInstance bracer && bracer.ArmorItem.ArmorType == ArmorType.Bracer;
            case EquipSlotType.Boots:
                return item is ArmorInstance boots && boots.ArmorItem.ArmorType == ArmorType.Boots;
            case EquipSlotType.WeaponLeft:
                return item is WeaponInstance;
            case EquipSlotType.WeaponRight:
                return item is WeaponInstance || item is ArmorInstance shield && shield.ArmorItem.ArmorType == ArmorType.Shield;
            default:
                return false;
        }
    }

    public void Equip(IItemInstance item, bool duplicate = false, bool placeholder = false)
    {
        _currentItem = item;
        IsDuplicate = duplicate;
        _isPlaceholder = placeholder;

        if (_itemIcon != null)
        {
            if (placeholder && item != null)
            {
                _itemIcon.sprite = item.ItemData.Icon; // можем сделать особую иконку, если хочешь
            }
            else
            {
                _itemIcon.sprite = item?.ItemData.Icon;
            }
            _itemIcon.enabled = item != null;
        }

        OnItemChanged?.Invoke();
    }

    public override void Clear()
    {
        base.Clear();
        IsDuplicate = false;
        _isPlaceholder = false;
        OnItemChanged?.Invoke();
    }
    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        if (eventData.clickCount == 1)
        {
            // ”правление выделением только через InventoryUI
            if (_inventoryUI != null)
            {
                _inventoryUI.SelectSlot(this);
            }
        }

    }

    // ќтпускаем предмет с экипировки (в инвентарь или в пустое место)
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedIcon != null)
            Destroy(_draggedIcon.gameObject);

        if (_currentItem == null)
            return;

        // ищем куда дропнули
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
            // попытка поместить предмет в инвентарь
            var old = targetSlot.ReplaceItemInInventory(_currentItem);

            if (old != null)
            {
                // если слот был зан€т, мен€ем местами
                Equip(old);
                _equipManager.EquipItem(old);
            }
            else
            {
                // если слот был пуст Ч снимаем предмет
                Clear();
                _equipManager.UnequipItem(SlotType);
            }

            targetSlot.SetSelected(false);
        }

        _draggedIcon = null;
    }

    // DROP: из инвентар€ -> экипировка
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var itemSlot = dragged.GetComponent<ItemSlotUI>();
        if (itemSlot == null) return;

        var item = itemSlot.ExtractItemFromInventory();
        if (item == null) return;

        if (!CanEquip(item))
        {
            itemSlot.ReplaceItemInInventory(item);
            return;
        }

        bool equipped = _equipManager.EquipItem(item);

        if (!equipped)
        {
            itemSlot.ReplaceItemInInventory(item);
            return;
        }

        itemSlot.SetSelected(false);
    }
}

