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
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public Transform CarTransform; // Reference to the car's transform
    public Transform CarTurnPoint;
    private bool IsMoving = false; // Flag to check if the car is moving
    public Rigidbody rb; // Reference to the car's Rigidbody
    public float speed = 10f; // Speed of the car
    public Transform ExitPoint; // Reference to the exit point of the car

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

        //makes the IsMoving flag true if the player is moving the car by checking the speed of the car, if the speed is greater than 0, then the car is moving
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }

        Vector3 movement = -transform.right * movementInput.y;
        //The car only turns when the player is moving forward or backward
        if (IsMoving)
        {
            transform.Rotate(Vector3.up * movementInput.x * speed * 0.1f);
        }

        rb.AddForce(movement * speed, ForceMode.Acceleration);

        //rb.MovePosition(
        //    rb.position + movement * speed * Time.fixedDeltaTime
        //);

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("Exiting car");
            isInCarAndDriving = false;
            PlayerRigidbody.isKinematic = false; // Make the player's Rigidbody non-kinematic to allow physics interactions
            playerMovement.enabled = true; // Enable player movement when exiting the car
            playerInput.enabled = true; // Enable player input when exiting the car
            CarPlayerInput.enabled = false; // Disable car player input
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

        PlayerRotationPoint.localRotation = Quaternion.Euler(0f, yaw, 0f);
        PlayerTransform.rotation = PlayerTargetTransform.transform.rotation; // Align the player's rotation with the car's rotation
        PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, PlayerTargetTransform.transform.position, 15 * Time.deltaTime);
    
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
