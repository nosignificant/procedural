using UnityEngine;
using System.Collections;
public class EngineLegs : MonoBehaviour
{
    [Header("Transform")]
    public Transform followingTarget;
    public Transform body;
    public Transform[] tipTargets;


    [Header("float")]

    public float followTriggerDist = 1f;
    public float stride = 5f;

    public LayerMask ground;

    [Header("private")]
    private bool isMoving = false;
    private Coroutine moveCoroutine;
    private Transform groundTarget;
    private Transform[] defaultLegOffsets;

    private Vector3 movingDir;

    void Update()
    {
        //방향 
        movingDir = (followingTarget.position - body.position).normalized;

        //기준이 되는 타겟까지의 거리
        float Dist = Vector3.Distance(body.position, followingTarget.position);

        //거리가 followTrigger보다 멀면 
        if (Dist > followTriggerDist)
        {
            //FootUtil.BodyFollowTarget(body, tipTargets[0], 5f, ground);
            if (!isMoving)
                moveCoroutine = StartCoroutine(moveForward());
        }
    }

    IEnumerator moveForward()
    {
        isMoving = true;

        float stepTime = 3f;
        float stepHeight = 3f;

        for (int i = 0; i < tipTargets.Length; i++)
        {
            Vector3 startPos = tipTargets[i].position;
            Vector3 targetPos = FootUtil.ForwardStride(tipTargets[i].position, movingDir, stride);

            //목표 위치는 몸통의 앞 + 보폭
            targetPos = FootUtil.SetTargetGround(targetPos, ground);

            //각 발을 앞으로 이동
            float t = 0f;

            while (t < 1f)
            {
                t += Time.fixedDeltaTime / stepTime;
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
                float heightCurve = Mathf.Sin(t * Mathf.PI) * stepHeight;

                tipTargets[i].position = currentPos + Vector3.up * heightCurve;
                yield return null;
            }

            tipTargets[i].position = targetPos;
        }
        isMoving = false;
        yield return new WaitForSeconds(1f);
    }
}
