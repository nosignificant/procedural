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
        //if (movingDir.magnitude < 0.1f)
        //stepPos += Random.insideUnitSphere;
        return stepPos;
    }

    //타겟이 멀리 떨어졌는지 아닌지 확인 
    public static bool isFootFarfromTarget(Vector3 nowPos, Vector3 targetPos, float criteria)
    {
        if (Vector3.Distance(nowPos, targetPos) > criteria)
            return true;
        else return false;
    }

    //발 떼기 
    public static IEnumerator MoveFoot(Transform start, Vector3 targetPos, float stepTime, float stepHeight)
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

    //발들 평균 위치로 몸 이동 
    public static IEnumerator MoveBody(Transform body, Transform[] feet, float moveDuration)
    {
        Vector3 avgPos = Vector3.zero;

        for (int i = 0; i < feet.Length; i++)
            avgPos += feet[i].position;

        avgPos /= feet.Length;
        avgPos.y += body.position.y;
        Vector3 startPos = body.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / moveDuration;
            body.position = Vector3.Lerp(startPos, avgPos, t);

            yield return null;

        }
        body.position = avgPos;
    }

    public static void BodyFollowTarget(Transform body, Transform target, float stride, LayerMask ground)
    {
        Vector3 movingDir = (target.position - body.position).normalized;
        Vector3 predictPos = movingDir * stride + body.position;

        body.position = Vector3.Lerp(body.position, predictPos, Time.deltaTime);
        Vector3 dir = (target.position - body.position).normalized;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            body.rotation = Quaternion.Slerp(body.rotation, lookRot, Time.deltaTime * 5f);
        }
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