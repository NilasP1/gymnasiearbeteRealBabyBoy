using System.Collections;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public bool isInCarAndDriving = false; // Flag to check if the player is in the car and driving
    public Transform PlayerTransform; // Reference to the player's transform
    public Rigidbody PlayerRigidbody; // Reference to the player's Rigidbody
    public GameObject TargetTransform;
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public Transform CarTransform; // Reference to the car's transform

    private void FixedUpdate()
    {
        if (isInCarAndDriving)
        {
            PlayerRigidbody.isKinematic = true; // Make the player's Rigidbody kinematic to prevent physics interactions
            playerMovement.enabled = false; // Disable player movement when in the car
            PlayerTransform.position = Vector3.Lerp(PlayerTransform.position, TargetTransform.transform.position, 0.1f);
            PlayerTransform.rotation = TargetTransform.transform.rotation; // Align the player's rotation with the car's rotation
        }
    }
}
