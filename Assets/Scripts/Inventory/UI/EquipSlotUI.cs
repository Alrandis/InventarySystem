using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

public class EquipSlotUI : MonoBehaviour, IDropHandler
{
    // Существующие поля
    [SerializeField] private Image _itemIcon;
    public EquipSlotType SlotType { get; private set; }
    public IItemInstance CurrentItem { get; private set; }
    public bool IsDuplicate { get; private set; } = false;

    public event System.Action OnItemChanged;

    public void Init(EquipSlotType type)
    {
        SlotType = type;
        Clear();
    }

    public bool CanEquip(IItemInstance item)
    {
        if (item == null) return false;

        switch (SlotType)
        {
            case EquipSlotType.Helmet:
                return item is ArmorInstance armor && armor.ArmorItem.ArmorType == ArmorType.Helmet;
            case EquipSlotType.Torso:
                return item is ArmorInstance torso && torso.ArmorItem.ArmorType == ArmorType.Torso;
            case EquipSlotType.Bracer:
                return item is ArmorInstance bracer && bracer.ArmorItem.ArmorType == ArmorType.Bracer;
            case EquipSlotType.Boots:
                return item is ArmorInstance boots && boots.ArmorItem.ArmorType == ArmorType.Boots;
            case EquipSlotType.Shield:
                return item is ArmorInstance shield && shield.ArmorItem.ArmorType == ArmorType.Shield;
            case EquipSlotType.WeaponLeft:
            case EquipSlotType.WeaponRight:
                return item is WeaponInstance;
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

    // Этот метод вызывается, когда предмет перетаскивается на слот
    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var itemSlot = dragged.GetComponent<ItemSlotUI>();
        if (itemSlot == null) return;

        var item = itemSlot.CurrentItem;

        if (!CanEquip(item)) return;

        // Если на этом слоте уже что-то есть, возвращаем в инвентарь
        if (CurrentItem != null)
        {
            itemSlot.SetItem(CurrentItem);
        }

        // Ставим предмет в слот экипировки
        Equip(item);
        itemSlot.Clear(); // очищаем оригинальный слот
    }
}

