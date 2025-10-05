using UnityEngine;

public class InventoryControllerUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _pickupHintPanel;
    [SerializeField] private GameObject _toolTipPanel;

    private bool _isOpen = false;

    private void OnEnable()
    {
        // ������������� �� ������� �����
        PauseManager.Instance.OnPauseChanged += HandlePauseChanged;
    }

    private void OnDisable()
    {
        // ������������ �� ������� �����
        PauseManager.Instance.OnPauseChanged -= HandlePauseChanged;
    }

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
            OpenInventory();
            // ������ ���� �� �����
            PauseManager.Instance.SetPause(true);
            _pickupHintPanel.SetActive(false);
        }
        else
        {
            CloseInventory();
            // ������� �����
            PauseManager.Instance.SetPause(false);
            _toolTipPanel.SetActive(false);
        }
    }

    private void OpenInventory()
    {
        // ���������� ������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        // �������� ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // ����� ��� ��������� �������� ������� �����
    private void HandlePauseChanged(bool isPaused)
    {
        _inventoryPanel.SetActive(isPaused);
        _isOpen = isPaused;

        // ���������� ��������
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
}
