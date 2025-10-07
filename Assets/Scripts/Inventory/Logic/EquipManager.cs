using UnityEngine;
using System.Collections.Generic;
using System;

public class EquipManager : MonoBehaviour
{
    [SerializeField] private EquipSlotUI[] _slots;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private PlayerStatsUI _statsUI;

    private Dictionary<EquipSlotType, EquipSlotUI> _slotDict;
    public IReadOnlyList<EquipSlotUI> Slots => _slots;
    public event Action OnEquipmentChanged;

    private void Awake()
    {
        _slotDict = new Dictionary<EquipSlotType, EquipSlotUI>();
        foreach (var slot in _slots)
        {
            slot.Init(this);
            _slotDict[slot.SlotType] = slot;
            slot.OnItemChanged += RecalculateStats;
        }
    }

    public void ForceEquip(EquipSlotType slotType, IItemInstance item)
    {
        if (_slotDict == null)
        {
            Debug.LogError("EquipManager: _slotDict == null (Awake() не вызван?)");
            return;
        }

        if (!_slotDict.TryGetValue(slotType, out var slot))
        {
            Debug.LogError($"EquipManager: слот {slotType} не найден в _slotDict!");
            return;
        }

        slot.Equip(item);
    }

    public bool EquipItem(IItemInstance item)
    {
        if (item == null) return false;

        if (item is ArmorInstance armor)
        {
            foreach (var slot in _slots)
            {
                if (slot.CanEquip(item))
                {
                    if (slot.CurrentItem != null)
                    {
                        _inventory.AddItem(slot.CurrentItem);
                        slot.Clear();
                    }

                    slot.Equip(item);
                    RecalculateStats();
                    OnEquipmentChanged?.Invoke();
                    return true;
                }
            }
        }
        else if (item is WeaponInstance weapon)
        {
            if (weapon.WeaponType.IsTwoHanded)
            {
                return EquipTwoHandedWeapon(weapon);
            }
            else
            {
                // обычна€ логика одноручного оружи€
                var left = _slotDict[EquipSlotType.WeaponLeft];
                var right = _slotDict[EquipSlotType.WeaponRight];

                // если в руках было двуручное Ч снимаем
                if ((left.CurrentItem is WeaponInstance lw && lw.WeaponType.IsTwoHanded) ||
                    (right.CurrentItem is WeaponInstance rw && rw.WeaponType.IsTwoHanded))
                {
                    if (left.CurrentItem != null && !left.IsDuplicate) _inventory.AddItem(left.CurrentItem);
                    if (right.CurrentItem != null && !right.IsDuplicate) _inventory.AddItem(right.CurrentItem);
                    left.Clear();
                    right.Clear();
                }

                // найти свободный слот
                EquipSlotUI targetSlot = left.CurrentItem == null ? left :
                                          right.CurrentItem == null ? right : left;

                // если слот зан€т Ч возвращаем старое в инвентарь
                if (targetSlot.CurrentItem != null) _inventory.AddItem(targetSlot.CurrentItem);

                targetSlot.Equip(weapon);
                RecalculateStats();
                OnEquipmentChanged?.Invoke();
                return true;
            }
        }

        Debug.LogWarning("Ќет подход€щего слота дл€ этого предмета");
        return false;
    }

    public bool EquipTwoHandedWeapon(WeaponInstance weapon)
    {
        var left = _slotDict[EquipSlotType.WeaponLeft];
        var right = _slotDict[EquipSlotType.WeaponRight];

        // возвращаем предметы из обеих рук в инвентарь
        if (left.CurrentItem != null && !left.IsDuplicate) _inventory.AddItem(left.CurrentItem);
        if (right.CurrentItem != null && !right.IsDuplicate) _inventory.AddItem(right.CurrentItem);

        left.Clear();
        right.Clear();

        // экипируем оружие в "правый" слот
        right.Equip(weapon);

        // в левый слот ставим визуальную заглушку
        left.Equip(weapon, duplicate: true, placeholder: true);

        RecalculateStats();
        OnEquipmentChanged?.Invoke();
        return true;
    }


    public void UnequipItem(EquipSlotType slotType)
    {
        if (!_slotDict.TryGetValue(slotType, out var slot) || slot.CurrentItem == null)
            return;

        var item = slot.CurrentItem;

        if (item is WeaponInstance weapon && weapon.WeaponType.IsTwoHanded)
        {
            var left = _slotDict[EquipSlotType.WeaponLeft];
            var right = _slotDict[EquipSlotType.WeaponRight];

            // возвращаем только оригинал (не дубликат)
            if (!left.IsDuplicate && left.CurrentItem != null)
            {
                _inventory.AddItem(left.CurrentItem);
            }
            else if (!right.IsDuplicate && right.CurrentItem != null)
            {
                _inventory.AddItem(right.CurrentItem);
            }

            // очищаем оба
            left.Clear();
            right.Clear();
        }
        else
        {
            _inventory.AddItem(item);
            slot.Clear();
        }

        RecalculateStats();
        OnEquipmentChanged?.Invoke();
    }

    private void RecalculateStats()
    {
        int totalArmor = 10;
        int totalDamage = 0;

        foreach (var slot in _slots)
        {
            if (slot.CurrentItem == null || slot.IsDuplicate) continue;

            if (slot.CurrentItem is ArmorInstance armor)
                totalArmor += armor.Defense;
            else if (slot.CurrentItem is WeaponInstance weapon)
                totalDamage += weapon.Damage;
        }

        _statsUI.UpdateStats(totalArmor, totalDamage);
    }
}
