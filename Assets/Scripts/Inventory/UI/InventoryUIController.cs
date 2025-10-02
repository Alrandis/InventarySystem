using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;

    private bool _isOpen = false;

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
            // Открываем инвентарь
            OpenInventory();
        }
        else
        {
            // Закрываем инвентарь
            CloseInventory();
        }
    }

    private void OpenInventory()
    {
        // Ставим игру на паузу
        Time.timeScale = 0f;

        // Показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        // Возвращаем нормальное время
        Time.timeScale = 1f;

        // Скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
