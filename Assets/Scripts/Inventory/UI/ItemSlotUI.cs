using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// Каждая ячейка инвентаря
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _itemIcon;         // Слот для иконки
    [SerializeField] private TextMeshProUGUI _stackText; // Текст для количества в стеке
    [SerializeField] private GameObject _selectionHighlight; // Подсветка выбранного предмета

    private IItemInstance _currentItem;               // Предмет, который хранится в этом слоте
    private bool _isSelected = false;

    public void SetItem(IItemInstance item)
    {
        _currentItem = item;

        if (_currentItem != null)
        {
            _itemIcon.sprite = _currentItem.ItemData.Icon;
            _itemIcon.enabled = true;

            if (_currentItem is IStackable stackable && stackable.CurrentStack > 1)
            {
                _stackText.text = stackable.CurrentStack.ToString();
                _stackText.enabled = true;
            }
            else
            {
                _stackText.enabled = false;
            }
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        _currentItem = null;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
        _stackText.text = "";
        _stackText.enabled = false;
        Deselect();
    }

    public void Select()
    {
        _isSelected = true;
        _selectionHighlight.SetActive(true);
    }

    public void Deselect()
    {
        _isSelected = false;
        _selectionHighlight.SetActive(false);
    }

    // Клик по слоту
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        // Используем предмет, если он реализует IUsableItem
        if (_currentItem is IUsableItem usable)
        {
            usable.Use();
        }

        // Можно здесь добавить логику выделения
        if (!_isSelected)
            Select();
        else
            Deselect();
    }
}
