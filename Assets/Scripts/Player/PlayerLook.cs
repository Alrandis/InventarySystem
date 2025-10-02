using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _sensitivity = 2f;
    [SerializeField] private float _minY = -80f;
    [SerializeField] private float _maxY = 80f;

    private PlayerInput _input;
    private float _pitch;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        // Горизонталь (вращает весь объект игрока)
        transform.Rotate(Vector3.up * _input.LookAxis.x * _sensitivity);

        // Вертикаль (только камеру, ограничиваем)
        _pitch -= _input.LookAxis.y * _sensitivity;
        _pitch = Mathf.Clamp(_pitch, _minY, _maxY);

        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
