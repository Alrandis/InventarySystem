using UnityEngine;
using System.Collections.Generic;

public class EquipManager : MonoBehaviour
{
    [SerializeField] private EquipSlotUI[] _slots;
    [SerializeField] private Inventory _inventory;  // ������ �� ��������� (���������)
    [SerializeField] private PlayerStatsUI _statsUI; // UI ��� ������, ���������� �������������
    private Dictionary<EquipSlotType, EquipSlotUI> _slotDict;

    private void Awake()
    {
        _slotDict = new Dictionary<EquipSlotType, EquipSlotUI>();
        foreach (var slot in _slots)
        {
            _slotDict[slot.SlotType] = slot;
            slot.OnItemChanged += RecalculateStats;
        }
    }

    public bool EquipItem(IItemInstance item)
    {
        // ���� ���������� ����
        EquipSlotUI targetSlot = null;

        if (item is ArmorInstance armor)
        {
            foreach (var slot in _slots)
            {
                if (slot.CanEquip(item))
                {
                    targetSlot = slot;
                    break;
                }
            }
        }
        else if (item is WeaponInstance weapon)
        {
            // ��������� ��������� ���� ��� ��������� ��������� ������
            targetSlot = _slotDict[EquipSlotType.WeaponLeft].CurrentItem == null ?
                _slotDict[EquipSlotType.WeaponLeft] : _slotDict[EquipSlotType.WeaponRight];

            if (weapon.WeaponType.IsTwoHanded)
            {
                // ������� ��� ����� � ������ ������������� �����������
                _slotDict[EquipSlotType.WeaponLeft].Equip(weapon);
                _slotDict[EquipSlotType.WeaponRight].Equip(weapon, true); // duplicate
                RecalculateStats();
                return true;
            }
        }

        if (targetSlot != null)
        {
            targetSlot.Equip(item);
            return true;
        }

        Debug.LogWarning("��� ����������� ����� ��� ����� ��������");
        return false;
    }

    public void UnequipItem(EquipSlotType slotType)
    {
        if (!_slotDict.ContainsKey(slotType)) return;

        var slot = _slotDict[slotType];
        if (slot.CurrentItem == null) return;

        if (slot.CurrentItem is WeaponInstance weapon && weapon.WeaponType.IsTwoHanded)
        {
            _slotDict[EquipSlotType.WeaponLeft].Clear();
            _slotDict[EquipSlotType.WeaponRight].Clear();
        }
        else
        {
            slot.Clear();
        }
    }

    private void RecalculateStats()
    {
        int totalArmor = 10; // �������
        int totalDamage = 0;

        foreach (var slot in _slots)
        {
            if (slot.CurrentItem == null || slot.IsDuplicate == true) continue;

            if (slot.CurrentItem is ArmorInstance armor)
            {
                totalArmor += armor.Defense;
            }
            else if (slot.CurrentItem is WeaponInstance weapon)
            {
                totalDamage += weapon.Damage;
            }
        }

        _statsUI.UpdateStats(totalArmor, totalDamage);
    }
}
