using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // ������ ���������, ����� ����� ���� �������� � ������, � �����, � ����������
    private List<IItemInstance> _items = new();
    // ��������� �������� ���������� �� �������� ��� �� ����
    private InventorySortType _currentSort = InventorySortType.None;
    // ������������ ������ �� ���, ����� � ������ ���� �� ��� �� ��� �������� ���������, ����� ���� ������
    public IReadOnlyList<IItemInstance> Items => _items;

    // ������� ������� ����� ������� UI
    public event Action<IItemInstance> OnItemAdded;
    public event Action<IItemInstance> OnItemRemoved;
    public event Action<InventorySortType> OnInventorySorted;

    public void AddItem(IItemInstance itemToAdd)
    {
        _items.Add(itemToAdd);
        OnItemAdded?.Invoke(itemToAdd);
        Debug.Log($"�������� �������: {itemToAdd.ItemData.Name}");
    }

    public void RemoveItem(IItemInstance itemToRemove)
    {
        _items.Remove(itemToRemove);
        OnItemRemoved?.Invoke(itemToRemove);
        Debug.Log($"������ �������: {itemToRemove.ItemData.Name}");
    }

    public bool Contains(IItemInstance item)
    {
        return _items.Contains(item);
    }

    public void Sort(InventorySortType sortType)
    {
        _currentSort = sortType;

        switch (sortType)
        {
            case InventorySortType.ByName:
                _items.Sort((a, b) => string.Compare(a.ItemData.Name, b.ItemData.Name));
                break;

            case InventorySortType.ByItemType:
                _items.Sort((a, b) => a.ItemData.ItemType.CompareTo(b.ItemData.ItemType));
                break;

            case InventorySortType.None:
            default:
                break; 
        }
        OnInventorySorted?.Invoke(sortType);
    }
}

public enum InventorySortType
{
    None,
    ByName,
    ByItemType
}
