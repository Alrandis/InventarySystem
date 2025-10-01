using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // Храним интерфейс, чтобы можно было положить и оружие, и броню, и расходники
    private List<IItemInstance> _items = new();
    // Указываем критерий сортировки по алфавиту или по типу
    private InventorySortType _currentSort = InventorySortType.None;
    // Ограничиваем доступ из вне, чтобы в случае чего ни кто не мог изменить инвентарь, кроме него самого
    public IReadOnlyList<IItemInstance> Items => _items;

    // События которые будет слушать UI
    public event Action<IItemInstance> OnItemAdded;
    public event Action<IItemInstance> OnItemRemoved;
    public event Action<InventorySortType> OnInventorySorted;

    public void AddItem(IItemInstance itemToAdd)
    {
        if (itemToAdd is IStackable stackable)
        {
            // ищем уже существующий стак, который можно объединить
            foreach (var existingItem in _items)
            {
                if (existingItem is IStackable existingStack && existingStack.CanStackWith(stackable))
                {
                    existingStack.AddToStack(stackable.CurrentStack);
                    OnItemAdded?.Invoke(existingItem);
                    Debug.Log($"Добавлено в стек: {itemToAdd.ItemData.Name}, новый размер стека: {existingStack.CurrentStack}");
                    return;
                }
            }
        }

        // Если не стекуемый или не нашли подходящий стак, добавляем как новый элемент
        _items.Add(itemToAdd);
        OnItemAdded?.Invoke(itemToAdd);
        Debug.Log($"Добавлен предмет: {itemToAdd.ItemData.Name}");
    }

    public void RemoveItem(IItemInstance itemToRemove, int amount = 1)
    {
        if (itemToRemove is IStackable stackable)
        {
            stackable.AddToStack(-amount); // уменьшаем стек
            if (stackable.CurrentStack <= 0)
            {
                _items.Remove(itemToRemove);
                OnItemRemoved?.Invoke(itemToRemove);
                Debug.Log($"Удален предмет: {itemToRemove.ItemData.Name}");
            }
            else
            {
                OnItemRemoved?.Invoke(itemToRemove); // можно уведомлять и о сокращении стека
                Debug.Log($"Уменьшен стек: {itemToRemove.ItemData.Name}, осталось: {stackable.CurrentStack}");
            }
            return;
        }

        // Для не-стекуемых просто удаляем
        _items.Remove(itemToRemove);
        OnItemRemoved?.Invoke(itemToRemove);
        Debug.Log($"Удален предмет: {itemToRemove.ItemData.Name}");
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
                    // сравниваем имена
                    int result = string.Compare(a.ItemData.Name, b.ItemData.Name, StringComparison.Ordinal);
                    if (result != 0) return result;

                    // если имена одинаковые, сравниваем стек (если есть)
                    int stackA = (a is IStackable sa) ? sa.CurrentStack : 1;
                    int stackB = (b is IStackable sb) ? sb.CurrentStack : 1;
                    return stackB.CompareTo(stackA); // большие стеки первыми
                });
                break;

            case InventorySortType.ByItemType:
                _items.Sort((a, b) =>
                {
                    int result = a.ItemData.ItemType.CompareTo(b.ItemData.ItemType);
                    if (result != 0) return result;

                    // если тип одинаковый, сравниваем стек
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
