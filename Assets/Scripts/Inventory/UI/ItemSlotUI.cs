using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private GameObject _selectionHighlight;

    private IItemInstance _currentItem;
    private bool _isSelected;

    // Drag visual
    public Image DraggedIcon { get; private set; }
    private Canvas _canvas;

    public int Index { get; private set; }

    public void Init(int index)
    {
        Index = index;
    }

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(IItemInstance item)
    {
        _currentItem = item;
        if (item == null)
        {
            Clear(); // Clear() внутри вызывает Deselect()
            return;
        }

        _itemIcon.sprite = item.ItemData.Icon;
        _itemIcon.enabled = true;

        if (item is IStackable stackable)
        {
            _stackText.text = stackable.CurrentStack.ToString();
            _stackText.enabled = true;
        }
        else
        {
            _stackText.enabled = false;
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

    public IItemInstance GetItem() => _currentItem;

    public void Select()
    {
        _isSelected = true;
        if (_selectionHighlight) _selectionHighlight.SetActive(true);
    }

    public void Deselect()
    {
        _isSelected = false;
        if (_selectionHighlight) _selectionHighlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        if (eventData.clickCount == 2 && _currentItem is IUsableItem usable)
        {
            usable.Use();

            // уведомляем Inventory сразу, чтобы UI обновился
            var inv = FindObjectOfType<Inventory>();
            if (inv != null) inv.HandleItemUsed(_currentItem);

        }

        if (!_isSelected) Select();
        else Deselect();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        // создаём визуал и передаём в InventoryUI
        DraggedIcon = new GameObject("DraggedIcon").AddComponent<Image>();
        DraggedIcon.raycastTarget = false;
        DraggedIcon.sprite = _itemIcon.sprite;
        DraggedIcon.transform.SetParent(_canvas.transform, false);
        DraggedIcon.transform.SetAsLastSibling();

        // масштаб/размер
        DraggedIcon.rectTransform.sizeDelta = _itemIcon.rectTransform.sizeDelta;

        var ui = FindObjectOfType<InventoryUI>();
        if (ui != null) ui.StartDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DraggedIcon != null)
            DraggedIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DraggedIcon != null)
            Destroy(DraggedIcon.gameObject);

        // raycast на UI под курсором
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        ItemSlotUI targetSlot = null;
        foreach (var r in results)
        {
            targetSlot = r.gameObject.GetComponent<ItemSlotUI>();
            if (targetSlot != null) break;
        }

        if (targetSlot != null)
        {
            var ui = FindObjectOfType<InventoryUI>();
            if (ui != null) ui.SwapSlots(this, targetSlot);
        }

        DraggedIcon = null;
    }
}
