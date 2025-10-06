using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] protected Image _itemIcon;
    [SerializeField] protected GameObject _selectionHighlight;
    [SerializeField] protected InventoryUI _inventoryUI;

    protected IItemInstance _currentItem;
    protected bool _isSelected;
    protected Canvas _canvas;
    protected Image _draggedIcon;


    public Image DraggedIcon { get { return _draggedIcon; } set { _draggedIcon = value; } }
    public IItemInstance CurrentItem => _currentItem;

    protected virtual void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _selectionHighlight.SetActive(false);
    }

    public virtual void Clear()
    {
        _currentItem = null;
        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }

    public virtual void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectionHighlight) 
            _selectionHighlight.SetActive(_isSelected);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentItem == null) return;

        // создаём визуал и передаём в InventoryUI
        _draggedIcon = new GameObject("DraggedIcon").AddComponent<Image>();
        _draggedIcon.raycastTarget = false;
        _draggedIcon.sprite = _itemIcon.sprite;
        _draggedIcon.transform.SetParent(_canvas.transform, false);
        _draggedIcon.transform.SetAsLastSibling();

        // масштаб/размер
        _draggedIcon.rectTransform.sizeDelta = _itemIcon.rectTransform.sizeDelta;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedIcon != null)
            _draggedIcon.transform.position = eventData.position;
    }
}
