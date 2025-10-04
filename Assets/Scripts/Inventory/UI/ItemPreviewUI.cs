using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Transform _statsContainer;
    [SerializeField] private GameObject _statPrefab;

    [SerializeField] private Transform _previewRoot;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private ModelRotator _modelRotator;

    [SerializeField] private GameObject _currentPreview;

    public void Show(IItemInstance item)
    {
        Clear();

        // Основная текстовая информация
        _titleText.text = item.ItemData.Name;
        _typeText.text = $"Тип предмета {GetItemType(item.ItemData.ItemType)}";
        _descriptionText.text = item.ItemData.Description;

        // Создаем 3D модель
        if (item.ItemData.Prefab != null)
        {
            _currentPreview = Instantiate(item.ItemData.Prefab, _previewRoot);
            Debug.Log($"Модель предмета создана {_currentPreview}");
            _currentPreview.layer = LayerMask.NameToLayer("ItemPreview");
            _currentPreview.transform.localPosition = Vector3.zero;
            _currentPreview.transform.localRotation = Quaternion.identity;
            _currentPreview.transform.localScale = Vector3.one;
            _modelRotator.TransformPrefab = _currentPreview.transform;
        }

        // Заполняем характеристики
        PopulateStats(item);
    }

    private void PopulateStats(IItemInstance item)
    {
        foreach (Transform child in _statsContainer)
            Destroy(child.gameObject);

        var stats = item.GetStats(); // Получаем динамически stats, нужно реализовать в IItemInstance

        foreach (var stat in stats)
        {
            var statGO = Instantiate(_statPrefab, _statsContainer);
            var statText = statGO.GetComponent<TextMeshProUGUI>();
            statText.text = $"{stat.Key}: {stat.Value}";
        }
    }

    public void Clear()
    {
        if (_currentPreview != null)
            Destroy(_currentPreview);

        _titleText.text = "";
        _typeText.text = "";
        _descriptionText.text = "";

        foreach (Transform child in _statsContainer)
            Destroy(child.gameObject);
    }

    private string GetItemType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Armor:
                return "броня";
            case ItemType.Potion:
                return "зелье";
            case ItemType.Weapon:
                return "оружие";
            case ItemType.Quest:
                return "квестовый предмет";
        }

        return "Тип не указан";
    }
}
