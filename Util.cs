using UnityEngine;


public static class Util
{
    public static float SmoothStep(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3.0f - 2.0f * t);
    }

    //Credit to Sam Hocevar of LolEngine
    //lolengine.net/blog/2013/09/21/picking-orthogonal-vector-combing-coconuts
    public static Vector3 Perpendicular(this Vector3 vec)
    {
        return Mathf.Abs(vec.x) > Mathf.Abs(vec.z) ? new Vector3(-vec.y, vec.x, 0f)
                                                   : new Vector3(0f, -vec.z, vec.y);
    }

    public static Vector3 ConstrainToNormal(this Vector3 direction, Vector3 normalDirection, float maxAngle)
    {
        if (maxAngle <= 0f)
            return normalDirection.normalized * direction.magnitude;
        if (maxAngle >= 180f)
            return direction;
        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(direction.normalized, normalDirection.normalized), -1f, 1f)) * Mathf.Rad2Deg;
        return Vector3.Slerp(direction.normalized, normalDirection.normalized, (angle - maxAngle) / angle) * direction.magnitude;
    }

    //레이캐스트로 땅 위치 찾기 - 아래로 
    public static Vector3 SetTargetGround(Vector3 movingDir, LayerMask ground, Transform transform)
    {
        Vector3 rayOrigin = movingDir + (Vector3.up * 10.0f);

        Debug.DrawRay(rayOrigin, Vector3.down * 20f, Color.red, 1.0f);
        bool found = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit rest, 500, ground);

        if (found) return rest.point;
        else return transform.position;
    }
}
