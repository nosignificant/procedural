using UnityEngine;
using System.Collections;

public static class FootUtil
{
    //레이캐스트로 땅 위치 찾기 - 아래로 
    public static Vector3 SetTargetGround(Vector3 targetPos, LayerMask ground)
    {
        Vector3 rayOrigin = targetPos + (Vector3.up * 50.0f);

        //Debug.DrawRay(rayOrigin, Vector3.down * 50f, Color.red, 1.0f);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 50.0f, ground))
        {
            return hit.point;
        }
        else
        {
            Vector3 fallback = targetPos;
            fallback.y = 0;
            return fallback;
        }
    }

    // 상하좌우 상관없이 가장 가까운 표면을 찾음
    public static Vector3 SetTargetNearest(Vector3 targetPos, LayerMask ground, float searchRadius = 20.0f)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPos, searchRadius, ground);

        if (colliders.Length == 0)
        {
            return targetPos;
        }

        Vector3 bestPoint = targetPos;
        float minDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            // ClosestPoint: 해당 콜라이더 표면 중 targetPos와 제일 가까운 점을 반환
            Vector3 point = col.ClosestPoint(targetPos);

            float dist = Vector3.Distance(targetPos, point);

            if (dist < minDistance)
            {
                minDistance = dist;
                bestPoint = point;
            }
        }

        return bestPoint;
    }

    public static Vector3 GetNormal(Vector3 targetSurfacePos, Vector3 referenceUpPos, LayerMask ground)
    {
        Vector3 dirToSurface = (targetSurfacePos - referenceUpPos).normalized;
        if (dirToSurface == Vector3.zero) dirToSurface = Vector3.down;

        Vector3 rayOrigin = targetSurfacePos - (dirToSurface * 2.0f);
        if (Physics.Raycast(rayOrigin, dirToSurface, out RaycastHit hit, 5.0f, ground))
        {
            return hit.normal;
        }
        return (referenceUpPos - targetSurfacePos).normalized;
    }


    //앞으로 발뻗을 위치 
    public static Vector3 ForwardStride(Vector3 nowPos, Vector3 movingDir, float stepDist)
    {
        float off = Random.Range(0.8f, 1.2f);
        Vector3 stepPos = nowPos + (movingDir * stepDist * off);
        return stepPos;
    }

    //발 떼기 
    public static IEnumerator lerpMove(Transform start, Vector3 targetPos, float stepTime, float stepHeight, Vector3 surfaceNormal)
    {
        Vector3 startPos = start.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / stepTime;

            // t가 1을 넘지 않게 안전장치
            if (t > 1f) t = 1f;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);

            // 0 ~ 1 ~ 0 의 사인파 곡선
            float heightCurve = Mathf.Sin(t * Mathf.PI) * stepHeight;

            // [중요] surfaceNormal 방향으로 들어올림
            start.position = currentPos + (surfaceNormal * heightCurve);

            yield return null;
        }

        start.position = targetPos;
    }

    public static IEnumerator lerpMove(Transform start, Vector3 targetPos, float stepTime, float stepHeight)
    {
        return lerpMove(start, targetPos, stepTime, stepHeight, Vector3.up);
    }

    public static void RotBody(Transform body, Vector3 moveDir)
    {
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

}