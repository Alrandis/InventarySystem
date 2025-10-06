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
            Debug.LogError("EquipManager: _slotDict == null (Awake() �� ������?)");
            return;
        }

        if (!_slotDict.TryGetValue(slotType, out var slot))
        {
            Debug.LogError($"EquipManager: ���� {slotType} �� ������ � _slotDict!");
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
                // ������� ������ ����������� ������
                var left = _slotDict[EquipSlotType.WeaponLeft];
                var right = _slotDict[EquipSlotType.WeaponRight];

                // ���� � ����� ���� ��������� � �������
                if ((left.CurrentItem is WeaponInstance lw && lw.WeaponType.IsTwoHanded) ||
                    (right.CurrentItem is WeaponInstance rw && rw.WeaponType.IsTwoHanded))
                {
                    if (left.CurrentItem != null && !left.IsDuplicate) _inventory.AddItem(left.CurrentItem);
                    if (right.CurrentItem != null && !right.IsDuplicate) _inventory.AddItem(right.CurrentItem);
                    left.Clear();
                    right.Clear();
                }

                // ����� ��������� ����
                EquipSlotUI targetSlot = left.CurrentItem == null ? left :
                                          right.CurrentItem == null ? right : left;

                // ���� ���� ����� � ���������� ������ � ���������
                if (targetSlot.CurrentItem != null) _inventory.AddItem(targetSlot.CurrentItem);

                targetSlot.Equip(weapon);
                RecalculateStats();
                OnEquipmentChanged?.Invoke();
                return true;
            }
        }

        Debug.LogWarning("��� ����������� ����� ��� ����� ��������");
        return false;
    }

    public bool EquipTwoHandedWeapon(WeaponInstance weapon)
    {
        var left = _slotDict[EquipSlotType.WeaponLeft];
        var right = _slotDict[EquipSlotType.WeaponRight];

        // ���������� �������� �� ����� ��� � ���������
        if (left.CurrentItem != null && !left.IsDuplicate) _inventory.AddItem(left.CurrentItem);
        if (right.CurrentItem != null && !right.IsDuplicate) _inventory.AddItem(right.CurrentItem);

        left.Clear();
        right.Clear();

        // ��������� ������ � "������" ����
        right.Equip(weapon);

        // � ����� ���� ������ ���������� ��������
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

            // ���������� ������ �������� (�� ��������)
            if (!left.IsDuplicate && left.CurrentItem != null)
            {
                _inventory.AddItem(left.CurrentItem);
            }
            else if (!right.IsDuplicate && right.CurrentItem != null)
            {
                _inventory.AddItem(right.CurrentItem);
            }

            // ������� ���
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
