
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepLogic : MonoBehaviour
{
    public List<GameObject> StepTargetPoints = new List<GameObject>();
    public List<GameObject> FeetColliders = new List<GameObject>();

    public float RaycastDistanceFromFeet = 2f;
    public LayerMask IgnoreFeet;

    [SerializeField] private float duration = 2f;
    [SerializeField] private float waveHeight = 3f;

    private List<bool> isLerping = new List<bool>();

    private void Start()
    {
        // Create one lerping state for each leg
        for (int i = 0; i < StepTargetPoints.Count; i++)
        {
            isLerping.Add(false);
        }
    }

    private Dictionary<GameObject, Vector3> hits = new();

    private void Update()
    {
        for (int i = 0; i < StepTargetPoints.Count; i++)
        {
            GameObject targetPoint = StepTargetPoints[i];

            Vector3 rayOrigin = targetPoint.transform.position
                                + Vector3.up * RaycastDistanceFromFeet;

            if (Physics.Raycast(rayOrigin, Vector3.down, // Raycast from the target point + slight y offset downwards to find the ground
                out RaycastHit hit, Mathf.Infinity))
            {
                if (FeetColliders[i] != hit.collider.gameObject) // If the raycast hit something that is not the feet collider, then we can assume that the leg needs to move
                {

                    Vector3 offsetY = Vector3.up * RaycastDistanceFromFeet;
                    Vector3 feetRayOrigin = FeetColliders[i].transform.position + offsetY;

                    if (Physics.Raycast(feetRayOrigin, Vector3.down,
                        out RaycastHit groundHit, Mathf.Infinity, IgnoreFeet))
                    {
                        Debug.DrawLine(feetRayOrigin, groundHit.point, Color.red);
                        if (hits.ContainsKey(FeetColliders[i]))
                        {
                            hits[FeetColliders[i]] = groundHit.point;
                        }
                        else
                        {
                            hits.Add(FeetColliders[i], groundHit.point);
                        }

                        if (isLerping.Where(x => x).Count() <= 4) // If there are less than 4 legs currently moving, then we can start moving this leg
                        {
                            StartCoroutine(WaveLerp(
                                targetPoint.transform,
                                groundHit.point,
                                i
                            ));
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        foreach (var hit in hits.Values)
        {
            Gizmos.DrawSphere(hit, 0.1f);
        }
    }

    public IEnumerator WaveLerp(Transform target, Vector3 finishPos, int index)
    {
        //yield return new WaitForSeconds(Random.value * 0.2f); // Add a small random delay to make the movement look more natural

        Vector3 startPos = target.position;
        float elapsed = 0f;

        isLerping[index] = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 pos = Vector3.Lerp(startPos, finishPos, smoothT);

            float lift = Mathf.Sin(t * Mathf.PI) * waveHeight;

            pos.y += lift;

            target.position = pos;

            yield return null;
        }

        target.position = finishPos;

        isLerping[index] = false;
    }
}
