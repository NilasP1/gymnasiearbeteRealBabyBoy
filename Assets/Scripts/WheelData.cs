using UnityEngine;

[System.Serializable]
public struct WheelData
{
    [Header("Model options")]
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;
    public Vector3 ScaleOffset;

    [Header("References")]
    public Transform Model;
    public WheelCollider Collider;

    [Header("State")]
    public bool Steerable;
    public bool Motorized;
}
