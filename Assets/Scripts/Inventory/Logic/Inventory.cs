using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum InventorySortType
{
    None,
    ByName,
    ByItemType
}

public class Inventory : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int _size = 24; // ����� ������ 3*8 = 24
    private IItemInstance[] _items;

    [SerializeField] private InventoryUI _inventoryUI;

    // ������� ����� ����������
    private InventorySortType _currentSort = InventorySortType.None;

    // �������:
    // (index, item) � item ����� ���� null (�������� ������ ����)
    public event Action<int, IItemInstance> OnItemChanged;
    public event Action<InventorySortType> OnInventorySorted;

    public int Size => _size;
    public IReadOnlyList<IItemInstance> Items => _items;

    private void Awake()
    {
        _items = new IItemInstance[_size];
    }

    // ��������� ������������� ��������
    public void HandleItemUsed(IItemInstance item)
    {
        int idx = IndexOf(item);

        if (item is IStackable stackable)
        {
            if (stackable.CurrentStack <= 0)
            {
                if (idx >= 0)
                {
                    
                    if (_inventoryUI != null)
                        _inventoryUI.ClearSelectionIfSlot(idx);

                    RemoveItemAt(idx); // ������� �������
                }
            }
            else
            {
                if (idx >= 0) OnItemChanged?.Invoke(idx, _items[idx]);
            }
        }
        else
        {
            if (idx >= 0) OnItemChanged?.Invoke(idx, _items[idx]);
        }
    }


    // �������� �������: ���������� ������ � ������������ ����, ����� � ������ null-����.
    public void AddItem(IItemInstance itemToAdd)
    {
        if (itemToAdd == null) return;

        // ���� ������� ���������
        if (itemToAdd is IStackable addingStack)
        {
            int remaining = addingStack.CurrentStack;

            // 1. ��������� � ������������ �������� �����
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] is IStackable existStack && existStack.CanStackWith(addingStack))
                {
                    int space = existStack.MaxStack - existStack.CurrentStack;
                    if (space > 0)
                    {
                        int toAdd = Mathf.Min(space, remaining);
                        existStack.AddToStack(toAdd);
                        remaining -= toAdd;
                        OnItemChanged?.Invoke(i, _items[i]);
                    }
                    if (remaining <= 0) return; // �� ���������
                }
            }

            // 2. ���� �������� �������� � ������ ����� ����� � ������ ������
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    int toPlace = Mathf.Min(addingStack.MaxStack, remaining);

                    // ������ ����� ��������� ����� ���� �� ����
                    IStackable newStack = CreateNewStackOfSameType(addingStack, toPlace);
                    _items[i] = (IItemInstance)newStack;

                    remaining -= toPlace;
                    OnItemChanged?.Invoke(i, _items[i]);

                    if (remaining <= 0) return;
                }
            }

            if (remaining > 0)
            {
                Debug.LogWarning($"��������� �����, �������� �� ����������� ���������: {remaining}");
            }

            return;
        }

        // ��-��������� ��������
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = itemToAdd;
                OnItemChanged?.Invoke(i, _items[i]);
                return;
            }
        }

        Debug.LogWarning("��������� �����, ������ �������� �������: " + itemToAdd.ItemData.Name);
    }

    private IStackable CreateNewStackOfSameType(IStackable original, int amount)
    {
        if (original is PotionInstance potion)
        {
            return new PotionInstance(potion.Data, amount);
        }

        // ���� ����� ������ IStackable ���� � �������� ����������
        throw new NotImplementedException("����������� ��� �����");
    }


    // �������� �� ������� (��� �������� ����� ����� ��� �������)
    public void RemoveItemAt(int index, int amount = 1)
    {
        if (index < 0 || index >= _items.Length) return;
        var item = _items[index];
        if (item == null) return;

        if (item is IStackable stack)
        {
            stack.RemoveFromStack(amount);
            if (stack.CurrentStack <= 0)
            {
                _items[index] = null;
                OnItemChanged?.Invoke(index, null);
                Debug.Log($"������ ������� (���� {index}): {item.ItemData.Name}");
            }
            else
            {
                OnItemChanged?.Invoke(index, _items[index]);
                Debug.Log($"�������� ���� (���� {index}): {item.ItemData.Name}, ��������: {stack.CurrentStack}");
            }
            return;
        }

        // ��-���������
        _items[index] = null;
        OnItemChanged?.Invoke(index, null);
        Debug.Log($"������ ������� (���� {index}): {item.ItemData.Name}");
    }

    public bool Contains(IItemInstance item)
    {
        return IndexOf(item) >= 0;
    }

    public int IndexOf(IItemInstance item)
    {
        return Array.IndexOf(_items, item);
    }

    // Swap ���� ��������
    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= _items.Length || indexB >= _items.Length) return;

        var tmp = _items[indexA];
        _items[indexA] = _items[indexB];
        _items[indexB] = tmp;

        OnItemChanged?.Invoke(indexA, _items[indexA]);
        OnItemChanged?.Invoke(indexB, _items[indexB]);
    }

    // ����������� �� fromIndex � toIndex (���� toIndex ������ � ������ move, ����� swap)
    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex >= _items.Length || toIndex >= _items.Length) return;
        if (_items[fromIndex] == null) return;

        if (_items[toIndex] == null)
        {
            _items[toIndex] = _items[fromIndex];
            _items[fromIndex] = null;
            OnItemChanged?.Invoke(fromIndex, null);
            OnItemChanged?.Invoke(toIndex, _items[toIndex]);
        }
        else
        {
            SwapItems(fromIndex, toIndex);
        }
    }

    // ����� ����� ����������� ������
    public void MergeStacks(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex >= _items.Length || toIndex >= _items.Length) return;

        var fromStack = _items[fromIndex] as IStackable;
        var toStack = _items[toIndex] as IStackable;

        if (fromStack == null || toStack == null) return;
        if (!fromStack.CanStackWith(toStack)) return;

        int availableSpace = toStack.MaxStack - toStack.CurrentStack;
        int toMove = Mathf.Min(fromStack.CurrentStack, availableSpace);

        if (toMove <= 0) return;

        // ������� �� ��������� � ��������� � �������
        fromStack.RemoveFromStack(toMove);
        toStack.AddToStack(toMove);

        // ���� �������� ���� ���� � ������� ���
        if (fromStack.CurrentStack <= 0)
            _items[fromIndex] = null;

        // ��������� UI
        OnItemChanged?.Invoke(fromIndex, _items[fromIndex]);
        OnItemChanged?.Invoke(toIndex, _items[toIndex]);
    }


    // ����������: ����� ��� ��-null ��������, ��������� � ����� �������, ����� null'�
    public void Sort(InventorySortType sortType)
    {
        _currentSort = sortType;

        // �������� ������������ ��������
        var present = _items.Where(x => x != null).ToList();

        switch (sortType)
        {
            case InventorySortType.ByName:
                present.Sort((a, b) => string.Compare(a.ItemData.Name, b.ItemData.Name, StringComparison.Ordinal));
                break;
            case InventorySortType.ByItemType:
                present.Sort((a, b) =>
                {
                    int r = a.ItemData.ItemType.CompareTo(b.ItemData.ItemType);
                    if (r != 0) return r;
                    return string.Compare(a.ItemData.Name, b.ItemData.Name, StringComparison.Ordinal);
                });
                break;
            case InventorySortType.None:
            default:
                // ������ �� ������
                break;
        }

        // ��������� ������ ������: ������� ���������������, ����� null'�
        var newItems = new IItemInstance[_items.Length];
        for (int i = 0; i < present.Count && i < newItems.Length; i++)
            newItems[i] = present[i];

        _items = newItems;

        // ����� ������� ��� ���� ������ (UI ���������)
        for (int i = 0; i < _items.Length; i++)
            OnItemChanged?.Invoke(i, _items[i]);

        OnInventorySorted?.Invoke(sortType);
    }

}
