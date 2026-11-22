
using UnityEngine;

public class AnimatedFog : MonoBehaviour
{
    [Header("Fog Colors")]
    public Color colorA = new Color(0.65f, 0.75f, 0.85f);
    public Color colorB = new Color(0.35f, 0.45f, 0.60f);

    [Header("Fog Density")]
    [Range(0f, 0.1f)]
    public float minDensity = 0.005f;

    [Range(0f, 0.1f)]
    public float maxDensity = 0.025f;

    [Header("Animation")]
    public float speed = 0.2f;

    [Header("Settings")]
    public bool animateColor = true;
    public bool animateDensity = true;

    private void Update()
    {
        // Smooth animation between 0 and 1
        float t = Mathf.PingPong(Time.time * speed, 1f);

        // Animate fog color
        if (animateColor)
        {
            RenderSettings.fogColor =
                Color.Lerp(colorA, colorB, t);
        }

        // Animate fog density
        if (animateDensity)
        {
            RenderSettings.fogDensity =
                Mathf.Lerp(minDensity, maxDensity, t);
        }
    }
}
