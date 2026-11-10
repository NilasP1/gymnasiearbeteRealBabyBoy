using UnityEngine;

public class BodyPlacement : MonoBehaviour
{
    public Transform Bodylocation;
    public float BodyDistanceFromGround = 1.0f;

    private void FixedUpdate()
    {
        //send raycast from the body location downwards to detect the distance from the body to the ground
        RaycastHit hit;
        if (Physics.Raycast(Bodylocation.position, Vector3.down, out hit))
        {
            float distanceToGround = hit.distance;

            if(distanceToGround < BodyDistanceFromGround || distanceToGround > BodyDistanceFromGround)
            {
                Bodylocation.position = new Vector3(
                    Bodylocation.position.x, 
                    hit.point.y + BodyDistanceFromGround, 
                    Bodylocation.position.z);
            }
        }
    }
}
