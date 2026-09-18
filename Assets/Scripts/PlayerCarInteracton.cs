using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarInteracton : MonoBehaviour
{

    [SerializeField] private CarController carScript;
    [SerializeField] private PlayerMovement playerScript;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float raydistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Keyboard.current.eKey.wasPressedThisFrame && carScript.IsInCar)
        {
            carScript.ExitCar();
        }

        if (Physics.Raycast(ray, out RaycastHit hit, raydistance))
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (hit.collider.CompareTag("Car"))
                {
                    carScript.EnterCar();
                }
            }
            Debug.DrawRay(ray.origin, ray.direction * raydistance, Color.red);
        }
    }

    public void OnEnterCar()
    {
        playerScript.enabled = false;
        playerScript.gameObject.GetComponent<PlayerInput>().enabled = false;
    }

    public void OnLeaveCar()
    {
        playerScript.enabled = true;
        playerScript.gameObject.GetComponent<PlayerInput>().enabled = true;
    }
}
