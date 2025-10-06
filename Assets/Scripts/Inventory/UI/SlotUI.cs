using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class SlotUI : MonoBehaviour
{
    [SerializeField] protected Image _itemIcon;
    [SerializeField] protected GameObject _selectionHighlight;
    [SerializeField] protected InventoryUI _inventoryUI;

    protected IItemInstance _currentItem;
    protected bool _isSelected;
    protected Canvas _canvas;

    public Image DraggedIcon { get; set; }
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


}
