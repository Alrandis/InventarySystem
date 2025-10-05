using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

public class EquipSlotUI : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ������������ ����
    [SerializeField] private Image _itemIcon;
    [SerializeField] private EquipSlotType _slotType;

    private Canvas _canvas;
    private Image _draggedIcon;

    public EquipSlotType SlotType { get; private set; }
    public IItemInstance CurrentItem { get; private set; }
    public bool IsDuplicate { get; private set; } = false;

    public event Action OnItemChanged;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }


    public void Init(EquipSlotType type)
    {
        _slotType = type;
        Clear();
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

    public void Equip(IItemInstance item, bool duplicate = false)
    {
        CurrentItem = item;
        IsDuplicate = duplicate;

        if (_itemIcon != null)
        {
            _itemIcon.sprite = item?.ItemData.Icon;
            _itemIcon.enabled = item != null;
        }

        OnItemChanged?.Invoke();
    }

    public void Clear()
    {
        CurrentItem = null;
        IsDuplicate = false;
        if (_itemIcon != null)
        {
            _itemIcon.sprite = null;
            _itemIcon.enabled = false;
        }
        OnItemChanged?.Invoke();
    }

    // ���� ������� �� ����������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurrentItem == null) return;

        _draggedIcon = new GameObject("DraggedEquipIcon").AddComponent<Image>();
        _draggedIcon.raycastTarget = false;
        _draggedIcon.sprite = _itemIcon.sprite;
        _draggedIcon.transform.SetParent(_canvas.transform, false);
        _draggedIcon.transform.SetAsLastSibling();
        _draggedIcon.rectTransform.sizeDelta = _itemIcon.rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedIcon != null)
            _draggedIcon.transform.position = eventData.position;
    }

    // ��������� ������� � ���������� (� ��������� ��� � ������ �����)
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedIcon != null)
            Destroy(_draggedIcon.gameObject);

        if (CurrentItem == null)
            return;

        // ���� ���� ��������
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
            // ������� ��������� ������� � ���������
            var old = targetSlot.ReplaceItemInInventory(CurrentItem);

            if (old != null)
            {
                // ���� ���� ��� �����, ������ �������
                Equip(old);
            }
            else
            {
                // ���� ���� ��� ���� � ������� �������
                Clear();
            }

            targetSlot.SetSelected(false);
        }

        _draggedIcon = null;
    }

    // DROP: �� ��������� -> ����������
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
            // ���������� �������, ���� �� ��������
            itemSlot.ReplaceItemInInventory(item);
            return;
        }

        var prev = CurrentItem;
        Equip(item);
        itemSlot.SetSelected(false);

        // ���� �� ���������� ��� ��� ������� � ���������� ��� � ���������
        if (prev != null)
        {
            itemSlot.ReplaceItemInInventory(prev);
        }
    }
}

