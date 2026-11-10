using System.Collections.Generic;
using UnityEngine;

public class StepLogic : MonoBehaviour
{
    public List<GameObject> StepTargetPoints = new List<GameObject>();
    public List<GameObject> FeetColliders = new List<GameObject>();
    public float RaycastDistanceFromFeet = 2f;
    public int currentListIndex = 0;
    public LayerMask IgnoreFeet;

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
                        // Move the target point to the ground position
                        targetPoint.transform.position = groundHit.point;
                        Debug.Log("Moved target point to ground position: " + groundHit.point);
                    }
                }
            }

            currentListIndex++;
        }
    }
}
