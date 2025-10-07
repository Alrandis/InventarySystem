using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveAxis { get; private set; }
    public Vector2 LookAxis { get; private set; }
    public bool InteractPressed { get; private set; }

    void Update()
    {
        // Движение (WASD)
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        MoveAxis = new Vector2(h, v).normalized;

        // Мышь (движение мыши по X/Y)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        LookAxis = new Vector2(mouseX, mouseY);

        // Взаимодействие (E)
        InteractPressed = Input.GetKeyDown(KeyCode.E);
    }
}