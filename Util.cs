using UnityEngine;


public static class Util
{
    public static float fromThis2Target(Vector3 posA, Vector3 posB)
    {
        return Vector3.Distance(posA, posB);
    }

    public static float SmoothStep(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3.0f - 2.0f * t);
    }
}
