using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _raydistance;
    [SerializeField] private Transform _cameraTransform;

    private CarController lastCarController = null;

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact pressed");
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

        if (lastCarController != null)
        {
            if (!lastCarController.IsInCar || lastCarController.IsInTransition) return;
            lastCarController.ExitCar();
            lastCarController = null;
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, _raydistance))
        {
            lastCarController = hit.collider.gameObject.GetComponentsInParent<CarController>().FirstOrDefault();
            lastCarController?.EnterCar(); // Enter car if script found otherwise do nothing
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_cameraTransform.position + _cameraTransform.forward, _cameraTransform.forward * _raydistance);
    }
}
