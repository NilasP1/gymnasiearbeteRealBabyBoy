using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarScript : MonoBehaviour
{
    private Vector2 lookInput;
    public bool isInCarAndDriving = false; // Flag to check if the player is in the car and driving
    public Transform PlayerTransform; // Reference to the player's transform
    public PlayerInput playerInput; // Reference to the PlayerInput component
    public PlayerInput CarPlayerInput;
    public Rigidbody PlayerRigidbody; // Reference to the player's Rigidbody
    public GameObject PlayerTargetTransform;
    public Transform PlayerRotationPoint;
    public Camera cam;
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public Transform CarTransform; // Reference to the car's transform
    public Rigidbody rb; // Reference to the car's Rigidbody
    public Transform ExitPoint; // Reference to the exit point of the car
    public float turnSpeed = 5f; // Speed at which the car turns

    public float maxSpeed = 20f;
    public float acceleration = 5f;
    public float deceleration = 0.5f;
    public float speed = 10f;

    private Vector2 movementInput;

    private float yaw = 0f;

    private void FixedUpdate()
    {
        if (!isInCarAndDriving)
            return;

        PlayerRigidbody.isKinematic = true; // Make the player's Rigidbody kinematic to prevent physics interactions
        playerMovement.enabled = false; // Disable player movement when in the car
        playerInput.enabled = false; // Disable player input when in the car
        CarPlayerInput.enabled = true; // Enable car player input

        Vector3 movement = -transform.right * movementInput.y;

        float turnAmount = movementInput.x * turnSpeed * Time.fixedDeltaTime;

        Quaternion rotation =
            Quaternion.Euler(0f, turnAmount, 0f);

        rb.AddForce(-transform.right * movementInput.y * speed, ForceMode.Acceleration);

        rb.MoveRotation(rb.rotation * rotation);
        //rb.MovePosition(
        //    rb.position + movement * speed * Time.fixedDeltaTime
        //);

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("Exiting car");
            isInCarAndDriving = false;
            PlayerRigidbody.isKinematic = false; // Make the player's Rigidbody non-kinematic to allow physics interactions
            playerMovement.enabled = true; // Enable player movement when exiting the car
            CarPlayerInput.enabled = false; // Disable car player input
            playerInput.enabled = true; // Enable player input when exiting the car
            PlayerTransform.position = ExitPoint.position; // Move the player to the exit point of the car
            PlayerTransform.rotation = ExitPoint.rotation; // Rotate the player to face the exit point of the car
        }
    }

    private void Update()
    {
        if (!isInCarAndDriving)
            return;

        float mouseX = lookInput.x * playerMovement.mouseSensitivity * Time.deltaTime;
        yaw += mouseX;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, rb.linearVelocity.magnitude * 1.5f + 50, 5 * Time.deltaTime); // Smoothly transition the camera's field of view to 60

        PlayerRotationPoint.localRotation = Quaternion.Euler(0f, yaw, 0f);
        PlayerTransform.rotation = PlayerTargetTransform.transform.rotation; // Align the player's rotation with the car's rotation
        //PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, PlayerTargetTransform.transform.position, 15 * Time.deltaTime);
        PlayerTransform.position = PlayerTargetTransform.transform.position; // Align the player's position with the car's position
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        Debug.Log("Car look input: " + lookInput);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log("Car movement input: " + movementInput);
    }
}
