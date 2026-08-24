using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movementInput;
    private Vector2 lookInput;
    public Rigidbody rb;
    public float speed = 5f;

    [SerializeField] private Transform cameraTransform;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;



    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump");
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement =
            transform.forward * movementInput.y +
            transform.right * movementInput.x;

        rb.MovePosition(
            rb.position + movement * speed * Time.fixedDeltaTime
        );
    }

    private void Update()
    {
        // Handle camera rotation based on look input
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    } 

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        Debug.Log($"Look Input: {lookInput}");
    }
}
