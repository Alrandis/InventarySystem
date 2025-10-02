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
        // ��������� ������� ������� I
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
            // ��������� ���������
            OpenInventory();
        }
        else
        {
            // ��������� ���������
            CloseInventory();
        }
    }

    private void OpenInventory()
    {
        // ������ ���� �� �����
        Time.timeScale = 0f;

        // ���������� ������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        // ���������� ���������� �����
        Time.timeScale = 1f;

        // �������� ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
