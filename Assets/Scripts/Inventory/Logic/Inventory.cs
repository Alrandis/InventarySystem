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
        if (itemToAdd is IStackable stackable)
        {
            // ���� ��� ������������ ����, ������� ����� ����������
            foreach (var existingItem in _items)
            {
                if (existingItem is IStackable existingStack && existingStack.CanStackWith(stackable))
                {
                    existingStack.AddToStack(stackable.CurrentStack);
                    OnItemAdded?.Invoke(existingItem);
                    Debug.Log($"��������� � ����: {itemToAdd.ItemData.Name}, ����� ������ �����: {existingStack.CurrentStack}");
                    return;
                }
            }
        }

        // ���� �� ��������� ��� �� ����� ���������� ����, ��������� ��� ����� �������
        _items.Add(itemToAdd);
        OnItemAdded?.Invoke(itemToAdd);
        Debug.Log($"�������� �������: {itemToAdd.ItemData.Name}");
    }

    public void RemoveItem(IItemInstance itemToRemove, int amount = 1)
    {
        if (itemToRemove is IStackable stackable)
        {
            stackable.AddToStack(-amount); // ��������� ����
            if (stackable.CurrentStack <= 0)
            {
                _items.Remove(itemToRemove);
                OnItemRemoved?.Invoke(itemToRemove);
                Debug.Log($"������ �������: {itemToRemove.ItemData.Name}");
            }
            else
            {
                OnItemRemoved?.Invoke(itemToRemove); // ����� ���������� � � ���������� �����
                Debug.Log($"�������� ����: {itemToRemove.ItemData.Name}, ��������: {stackable.CurrentStack}");
            }
            return;
        }

        // ��� ��-��������� ������ �������
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
                _items.Sort((a, b) =>
                {
                    // ���������� �����
                    int result = string.Compare(a.ItemData.Name, b.ItemData.Name, StringComparison.Ordinal);
                    if (result != 0) return result;

                    // ���� ����� ����������, ���������� ���� (���� ����)
                    int stackA = (a is IStackable sa) ? sa.CurrentStack : 1;
                    int stackB = (b is IStackable sb) ? sb.CurrentStack : 1;
                    return stackB.CompareTo(stackA); // ������� ����� �������
                });
                break;

            case InventorySortType.ByItemType:
                _items.Sort((a, b) =>
                {
                    int result = a.ItemData.ItemType.CompareTo(b.ItemData.ItemType);
                    if (result != 0) return result;

                    // ���� ��� ����������, ���������� ����
                    int stackA = (a is IStackable sa) ? sa.CurrentStack : 1;
                    int stackB = (b is IStackable sb) ? sb.CurrentStack : 1;
                    return stackB.CompareTo(stackA);
                });
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
