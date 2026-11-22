
using UnityEngine;

public class MoonRotation : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    void Update()
    {
        transform.Rotate(
            Vector3.right,
            rotationSpeed * Time.deltaTime
        );
    }
}
