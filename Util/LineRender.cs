using UnityEngine;

public class LineRender : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Draw(Transform[] points)
    {
        if (points != null && lineRenderer != null)
        {
            if (lineRenderer.positionCount != points.Length)
            {
                lineRenderer.positionCount = points.Length;
            }

            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i].position);
            }
        }
    }

}