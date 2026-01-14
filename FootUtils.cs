using UnityEngine;

public static class FootUtils
{
    //레이캐스트로 땅 위치 찾기 - 아래로 
    public static Vector3 SetTargetGround(Vector3 targetPos, LayerMask ground)
    {

        Vector3 rayOrigin = targetPos + (Vector3.up * 10.0f);

        Debug.DrawRay(rayOrigin, Vector3.down * 20f, Color.red, 1.0f);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 50.0f, ground))
        {
            return hit.point;
        }
        else
            return targetPos;
    }

    //앞으로 발뻗을 위치 
    public static Vector3 ForwardStride(Vector3 nowPos, Vector3 movingDir, float stepDist)
    {
        float off = Random.Range(0f, 1f);

        Vector3 stepPos = nowPos + (movingDir * stepDist * off);
        if (movingDir.magnitude < 0.1f)
            stepPos += Random.insideUnitSphere;
        return stepPos;
    }
    public static bool isFootFarfromTarget(Vector3 nowPos, Vector3 targetPos, float criteria)
    {
        if (Vector3.Distance(nowPos, targetPos) > criteria)
            return true;
        else return false;
    }
}