using UnityEngine;

public class ItemPickupController : MonoBehaviour
{
    [SerializeField] private float _pickupRange = 1f;
    [SerializeField] private LayerMask _pickupLayer;

    private Camera _camera;
    [SerializeField] private Inventory _inventory;

    private PickupItem _currentTarget;
    private Ray _ray;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        CheckForItem();

        if (_currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            _currentTarget.Pickup(_inventory);
            PickupHintUI.Instance.Hide();
            _currentTarget = null;
        }
    }

    private void CheckForItem()
    {
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // центр экрана
        if (Physics.Raycast(_ray, out RaycastHit hit, _pickupRange, _pickupLayer))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item != null)
            {
                if (_currentTarget != item)
                {
                    _currentTarget = item;
                    PickupHintUI.Instance.Show($"Нажмите [E], чтобы взять {item.ItemData.Name}");
                }
                return;
            }
        }

        // Если ничего не видим
        if (_currentTarget != null)
        {
            PickupHintUI.Instance.Hide();
            _currentTarget = null;
        }
    }
}
