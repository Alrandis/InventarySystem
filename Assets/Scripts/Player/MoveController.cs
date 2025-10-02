using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MoveController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private PlayerInput _input;
    private CharacterController _controller;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = transform.right * _input.MoveAxis.x +
                       transform.forward * _input.MoveAxis.y;

        _controller.Move(move * _moveSpeed * Time.deltaTime);
    }
}
