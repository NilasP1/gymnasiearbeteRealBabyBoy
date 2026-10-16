using UnityEngine;

public class CheckPlayerAgro : MonoBehaviour
{
    public bool PlayerInAgroRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInAgroRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInAgroRange = false;
        }
    }
}