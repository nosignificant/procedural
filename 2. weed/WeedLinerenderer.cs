using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform[] points;

    void Awake()
    {
        Weed3 weed = GetComponent<Weed3>();
        lineRenderer = GetComponent<LineRenderer>();

        if (weed != null) points = weed.parts;
        if (points != null) lineRenderer.positionCount = points.Length;
    }

    void Update()
    {
        if (points != null && lineRenderer != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i].position);
            }
        }
    }
}