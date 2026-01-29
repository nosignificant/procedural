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
    public float stride = 3f;

    public LayerMask ground;

    [Header("private")]
    private bool isMoving = false;
    private Coroutine moveCoroutine;
    private Transform groundTarget;
    private Vector3[] defaultLegOffsets;

    private Vector3 movingDir;

    void Start()
    {
        defaultLegOffsets = new Vector3[tipTargets.Length];
        for (int i = 0; i < tipTargets.Length; i++)
            defaultLegOffsets[i] = body.InverseTransformPoint(tipTargets[i].position);
    }

    void Update()
    {
        //방향 
        movingDir = (followingTarget.position - body.position).normalized;

        //기준이 되는 타겟까지의 거리
        float Dist = Vector3.Distance(body.position, followingTarget.position);

        //거리가 followTrigger보다 멀면 
        if (Dist > followTriggerDist)
            Move();
        else
            Stop();

    }
    void Move()
    {
        FootUtil.RotBody(body, movingDir);
        body.position = Vector3.Lerp(body.position, FootAvgPos(), Time.deltaTime * 5);
        if (!isMoving)
            moveCoroutine = StartCoroutine(moveForward());
    }

    void Stop()
    {
        StartCoroutine(ReturnToStance());

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        isMoving = false;
    }

    IEnumerator moveForward()
    {
        isMoving = true;

        float stepTime = 3f;
        float stepHeight = 3f;

        for (int i = 0; i < tipTargets.Length; i++)
        {
            Vector3 myIdealPos = body.TransformPoint(defaultLegOffsets[i]);
            Vector3 targetPos = FootUtil.ForwardStride(myIdealPos, movingDir, stride);

            //목표 위치는 몸통의 앞 + 보폭
            targetPos = FootUtil.SetTargetGround(targetPos, ground);

            yield return StartCoroutine(FootUtil.lerpMove(tipTargets[i], targetPos, stepTime, stepHeight));

            tipTargets[i].position = targetPos;
        }
        yield return new WaitForSeconds(0.1f);

        isMoving = false;
    }

    IEnumerator ReturnToStance()
    {
        isMoving = true;
        for (int i = 0; i < tipTargets.Length; i++)
        {
            Vector3 returnPos = body.TransformPoint(defaultLegOffsets[i]);
            returnPos = FootUtil.SetTargetGround(returnPos, ground);

            tipTargets[i].position = Vector3.Lerp(tipTargets[i].position, returnPos, Time.deltaTime * 5);
        }
        isMoving = false;

        yield return null;
    }


    Vector3 FootAvgPos()
    {
        Vector3 movedVector = Vector3.zero;

        for (int i = 0; i < tipTargets.Length; i++)
        {
            movedVector += tipTargets[i].position;
        }
        return movedVector / tipTargets.Length;

    }
}

