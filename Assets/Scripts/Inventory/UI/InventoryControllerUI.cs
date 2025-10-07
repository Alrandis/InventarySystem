using UnityEngine;

public class InventoryControllerUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _pickupHintPanel;
    [SerializeField] private GameObject _toolTipPanel;

    private bool _isOpen = false;

    private void OnEnable()
    {
        // Подписываемся на событие паузы
        PauseManager.Instance.OnPauseChanged += HandlePauseChanged;
    }

    private void OnDisable()
    {
        // Отписываемся от события паузы
        PauseManager.Instance.OnPauseChanged -= HandlePauseChanged;
    }

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Проверяем нажатие клавиши I
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        _isOpen = !_isOpen;
        _inventoryPanel.SetActive(_isOpen);

        if (_isOpen)
        {
            OpenInventory();
            // Ставим игру на паузу
            PauseManager.Instance.SetPause(true);
            _pickupHintPanel.SetActive(false);
        }
        else
        {
            CloseInventory();
            // Снимаем паузу
            PauseManager.Instance.SetPause(false);
            _toolTipPanel.SetActive(false);
        }
    }

    private void OpenInventory()
    {
        // Показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        // Скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // Метод для обработки внешнего события паузы
    private void HandlePauseChanged(bool isPaused)
    {
        _inventoryPanel.SetActive(isPaused);
        _isOpen = isPaused;

        // Управление курсором
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
}
