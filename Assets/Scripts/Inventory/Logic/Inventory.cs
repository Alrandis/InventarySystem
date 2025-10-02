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
    private int _size = 24; // можно задать 3*8 = 24
    private IItemInstance[] _items;

    [SerializeField] private InventoryUI _inventoryUI;

    // Текущий режим сортировки
    private InventorySortType _currentSort = InventorySortType.None;

    // События:
    // (index, item) — item может быть null (означает пустой слот)
    public event Action<int, IItemInstance> OnItemChanged;
    public event Action<InventorySortType> OnInventorySorted;

    public int Size => _size;
    public IReadOnlyList<IItemInstance> Items => _items;

    private void Awake()
    {
        _items = new IItemInstance[_size];
    }

    // Обработка использования предмета
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

                    RemoveItemAt(idx); // удаляем предмет
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


    // Добавить предмет: попытаемся встать в существующий стак, иначе в первый null-слот.
    public void AddItem(IItemInstance itemToAdd)
    {
        if (itemToAdd == null) return;

        // Если предмет стекуемый
        if (itemToAdd is IStackable addingStack)
        {
            int remaining = addingStack.CurrentStack;

            // 1. Добавляем в существующие неполные стеки
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
                    if (remaining <= 0) return; // всё добавлено
                }
            }

            // 2. Если остались предметы — создаём новые стеки в пустых слотах
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    int toPlace = Mathf.Min(addingStack.MaxStack, remaining);

                    // создаём новый экземпляр стека того же типа
                    IStackable newStack = CreateNewStackOfSameType(addingStack, toPlace);
                    _items[i] = (IItemInstance)newStack;

                    remaining -= toPlace;
                    OnItemChanged?.Invoke(i, _items[i]);

                    if (remaining <= 0) return;
                }
            }

            if (remaining > 0)
            {
                Debug.LogWarning($"Инвентарь полон, осталось не добавленных предметов: {remaining}");
            }

            return;
        }

        // Не-стекуемые предметы
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null)
            {
                _items[i] = itemToAdd;
                OnItemChanged?.Invoke(i, _items[i]);
                return;
            }
        }

        Debug.LogWarning("Инвентарь полон, нельзя добавить предмет: " + itemToAdd.ItemData.Name);
    }

    private IStackable CreateNewStackOfSameType(IStackable original, int amount)
    {
        if (original is PotionInstance potion)
        {
            return new PotionInstance(potion.Data, amount);
        }

        // если будут другие IStackable типы — добавить аналогично
        throw new NotImplementedException("Неизвестный тип стека");
    }


    // Удаление по индексу (для удаления части стека или целиком)
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
                Debug.Log($"Удален предмет (слот {index}): {item.ItemData.Name}");
            }
            else
            {
                OnItemChanged?.Invoke(index, _items[index]);
                Debug.Log($"Уменьшен стак (слот {index}): {item.ItemData.Name}, осталось: {stack.CurrentStack}");
            }
            return;
        }

        // не-стекуемый
        _items[index] = null;
        OnItemChanged?.Invoke(index, null);
        Debug.Log($"Удален предмет (слот {index}): {item.ItemData.Name}");
    }

    public bool Contains(IItemInstance item)
    {
        return IndexOf(item) >= 0;
    }

    public int IndexOf(IItemInstance item)
    {
        return Array.IndexOf(_items, item);
    }

    // Swap двух индексов
    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= _items.Length || indexB >= _items.Length) return;

        var tmp = _items[indexA];
        _items[indexA] = _items[indexB];
        _items[indexB] = tmp;

        OnItemChanged?.Invoke(indexA, _items[indexA]);
        OnItemChanged?.Invoke(indexB, _items[indexB]);
    }

    // Переместить из fromIndex в toIndex (если toIndex пустой — просто move, иначе swap)
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

    // Новый метод объединения стеков
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

        // Убираем из исходного и добавляем в целевой
        fromStack.RemoveFromStack(toMove);
        toStack.AddToStack(toMove);

        // Если исходный стак пуст — удаляем его
        if (fromStack.CurrentStack <= 0)
            _items[fromIndex] = null;

        // Обновляем UI
        OnItemChanged?.Invoke(fromIndex, _items[fromIndex]);
        OnItemChanged?.Invoke(toIndex, _items[toIndex]);
    }


    // Сортировка: берем все не-null элементы, сортируем и кладём сначала, затем null'ы
    public void Sort(InventorySortType sortType)
    {
        _currentSort = sortType;

        // собираем существующие предметы
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
                // ничего не делаем
                break;
        }

        // заполняем массив заново: сначала отсортированные, потом null'ы
        var newItems = new IItemInstance[_items.Length];
        for (int i = 0; i < present.Count && i < newItems.Length; i++)
            newItems[i] = present[i];

        _items = newItems;

        // вызов событий для всех слотов (UI обновится)
        for (int i = 0; i < _items.Length; i++)
            OnItemChanged?.Invoke(i, _items[i]);

        OnInventorySorted?.Invoke(sortType);
    }

}
