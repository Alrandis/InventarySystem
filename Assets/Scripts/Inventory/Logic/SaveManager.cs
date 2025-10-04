using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private Inventory _inventory;
    [SerializeField] private InventorySaveSystem _inventorySaveSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_inventorySaveSystem != null)
            _inventorySaveSystem.Load(); // Загружаем инвентарь при старте сцены

        if (_inventory != null)
        {
            // Подписываемся на изменения инвентаря для авто-сохранения
            _inventory.OnItemChanged += (_, __) => _inventorySaveSystem?.Save();
            _inventory.OnInventorySorted += (_) => _inventorySaveSystem?.Save();
            
            _inventory.OnInventoryLayoutChanged += () => _inventorySaveSystem?.Save();
        }
    }

    public void SaveAll()
    {
        _inventorySaveSystem?.Save();
        // В будущем можно добавить другие системы (например, прогресс игрока, настройки и т.д.)
    }

    public void LoadAll()
    {
        _inventorySaveSystem?.Load();
    }
}
