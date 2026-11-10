using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepLogic : MonoBehaviour
{
    public List<GameObject> StepTargetPoints = new List<GameObject>();
    public List<GameObject> FeetColliders = new List<GameObject>();
    public float RaycastDistanceFromFeet = 2f;
    public int currentListIndex = 0;
    public LayerMask IgnoreFeet;

    [SerializeField] private float duration = 2f;
    [SerializeField] private float waveHeight = 3f;

    private void FixedUpdate()
    {
        currentListIndex = 0;

        //fire a raycast from above the target point downwards to detect if the raycast hits the ground or the feet collider
        foreach (GameObject targetPoint in StepTargetPoints)
        {
            RaycastHit hit;
            Vector3 rayOrigin = targetPoint.transform.position + Vector3.up * RaycastDistanceFromFeet;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, RaycastDistanceFromFeet * 2))
            {
                // Check if the hit object is a feet collider
                if (FeetColliders.Contains(hit.collider.gameObject))
                {
                    Debug.Log("Raycast hit a feet collider: " + hit.collider.gameObject.name);
                }
                else
                {
                    Debug.Log("Raycast hit something else: " + hit.collider.gameObject.name);

                    // Move the target point to the feet collider's position by fireing another raycast from the feet collider's position downwards to find the ground
                    RaycastHit groundHit;
                    Vector3 feetRayOrigin = FeetColliders[currentListIndex].transform.position + Vector3.up * RaycastDistanceFromFeet;
                    if (Physics.Raycast(feetRayOrigin, Vector3.down, out groundHit, RaycastDistanceFromFeet * 2, IgnoreFeet))
                    {
                       

                         StartCoroutine(WaveLerp(targetPoint.transform, groundHit.point));

                        Debug.Log("Moved target point to ground position: " + groundHit.point);
                    }
                }
            }

            currentListIndex++;
        }
    }

    public IEnumerator WaveLerp(Transform target, Vector3 finishPos)
    {
        Vector3 startPos = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Normal movement from start to finish
            Vector3 pos = Vector3.Lerp(startPos, finishPos, t);

            // Wave offset (starts and ends at zero)
            float wave = Mathf.Sin(t * Mathf.PI) * waveHeight;

            // Apply wave vertically
            pos.y += wave;

            target.position = pos;

            yield return null;
        }

        // Ensure exact finishing position
        target.position = finishPos;
    }
}
