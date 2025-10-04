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
            _inventorySaveSystem.Load(); // ��������� ��������� ��� ������ �����

        if (_inventory != null)
        {
            // ������������� �� ��������� ��������� ��� ����-����������
            _inventory.OnItemChanged += (_, __) => _inventorySaveSystem?.Save();
            _inventory.OnInventorySorted += (_) => _inventorySaveSystem?.Save();
            
            _inventory.OnInventoryLayoutChanged += () => _inventorySaveSystem?.Save();
        }
    }

    public void SaveAll()
    {
        _inventorySaveSystem?.Save();
        // � ������� ����� �������� ������ ������� (��������, �������� ������, ��������� � �.�.)
    }

    public void LoadAll()
    {
        _inventorySaveSystem?.Load();
    }
}
