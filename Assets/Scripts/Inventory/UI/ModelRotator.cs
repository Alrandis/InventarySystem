using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

/// <summary>
/// Управляет вращением 3D-модели при наведении и удержании мыши.
/// Может использоваться в любой системе превью.
/// </summary>
public class ModelRotator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [SerializeField] private float autoRotationSpeed = 20f;
    [SerializeField] private float manualRotationSpeed = 5f;

    private bool _isPointerOver;
    private bool _isDragging;
    private float _inertia;
    private float _lastDeltaX;
    private Transform _transformPrefab;
    public Transform TransformPrefab 
    {
        get { return _transformPrefab; }
        set { _transformPrefab = value; }
    }

    private void Update()
    {
        if (!_isDragging && _transformPrefab != null)
        {
            // Постепенное вращение с инерцией или автоповорот
            float speed = Mathf.Abs(_inertia) > 0.01f ? _inertia : autoRotationSpeed * Time.unscaledDeltaTime;
            _transformPrefab.Rotate(Vector3.up, speed, Space.World);

            // Плавное затухание инерции
            _inertia = Mathf.Lerp(_inertia, 0, Time.unscaledDeltaTime * 2f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
        _isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPointerOver) return;

        _isDragging = true;
        float rotationY = -eventData.delta.x * manualRotationSpeed * Time.unscaledDeltaTime;
        _transformPrefab.Rotate(Vector3.up, rotationY, Space.World);

        // Вертикальное вращение (ограниченное)
        float rotationX = eventData.delta.y * manualRotationSpeed * Time.unscaledDeltaTime;
        _transformPrefab.Rotate(Vector3.right, rotationX, Space.World);

        _lastDeltaX = rotationY;
        _inertia = rotationY * 5f; // добавляем эффект инерции
    }
}
