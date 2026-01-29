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

    //앞으로 발뻗을 위치 
    public static Vector3 ForwardStride(Vector3 nowPos, Vector3 movingDir, float stepDist)
    {
        float off = Random.Range(0.8f, 1.2f);
        Vector3 stepPos = nowPos + (movingDir * stepDist * off);
        return stepPos;
    }

    //발 떼기 
    public static IEnumerator lerpMove(Transform start, Vector3 targetPos, float stepTime, float stepHeight)
    {
        Vector3 startPos = start.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / stepTime;
            Vector3 currentPos = Vector3.Lerp(start.position, targetPos, t);
            float heightCurve = Mathf.Sin(t * Mathf.PI) * stepHeight;

            start.position = currentPos + Vector3.up * heightCurve;
            yield return null;
        }

        start.position = targetPos;
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