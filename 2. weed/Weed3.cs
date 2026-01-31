
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weed3 : MonoBehaviour
{
    public LayerMask ground;

    [Header("position")]

    //발과 머리는 직접 코드 상으로 이동시키지 않는다.
    //tipTarget과 target을 이동시킨 후 따라다니게 만든다.
    public Transform foot;
    //top은 그냥 이동용으로만 쓰자 
    public Transform top;

    public Transform[] parts;
    public Transform tipTarget;
    private Vector3 targetPos;
    public Transform target;

    [Header("offsets")]

    public float stride = 10.0f;

    public float stepThreshold = 10f;
    private float offset;

    private float topToTargetDist;

    [Header("moving dir")]
    private Vector3 movingDir;
    private bool isMoving = false;

    void Start()
    {
        if (tipTarget != null)
        {
            targetPos = tipTarget.position;
            targetPos = FootUtil.SetTargetGround(targetPos, ground);
            tipTarget.position = targetPos;
        }
    }

    void Update()
    {
        foot.position = Vector3.Lerp(foot.position, tipTarget.position, Time.deltaTime * 10f);
        top.position = target.position;

        //1. 방향: 따라다닐 타겟과 발 
        movingDir = (target.position - foot.position).normalized;

        //2. 몸정렬
        bodyFABRIK();

        //3. 타겟까지 거리 구하고 발을 뗄지 말지 결정
        topToTargetDist = Vector3.Distance(top.transform.position, targetPos);

        if (topToTargetDist > stepThreshold)
            Move();
        else Stop();
    }

    void Move()
    {
        if (!isMoving)
            StartCoroutine(MoveFoot());
    }

    void Stop()
    {
        isMoving = false;
        StopCoroutine(MoveFoot());
    }

    //fabrik 몸 정렬 알고리즘 
    void bodyFABRIK()
    {
        offset = topToTargetDist / (parts.Length - 1);

        for (int i = parts.Length - 2; i > 0; i--)
        {
            Transform current = parts[i];
            Transform lower = parts[i + 1];

            //정렬되어야할 방향: 머리 - 발 끝 
            Vector3 footTotopDir = (top.position - tipTarget.position).normalized;

            //내 바로 아래 놈 위치
            Vector3 dir = (current.position - lower.position).normalized;
            //방향 정렬
            Vector3 finalDir = Vector3.Lerp(dir, footTotopDir, 0.5f).normalized;

            //약간 곡선으로 만들기
            float t = (float)i / (parts.Length - 1);

            float curve = Mathf.Sin(t * Mathf.PI);
            Vector3 sagVector = (Vector3.down) + (movingDir);

            //최종 위치
            Vector3 finalPos = lower.transform.position + (finalDir * offset);
            finalPos += sagVector * curve;
            //이동
            current.position = Vector3.Lerp(current.position, finalPos, Time.deltaTime * 20f);
            current.LookAt(lower.transform);
        }
    }

    IEnumerator MoveFoot()
    {
        isMoving = true;

        targetPos = FootUtil.ForwardStride(foot.position, movingDir, stride);
        targetPos = FootUtil.SetTargetGround(targetPos, ground);

        float stepTime = 4f;
        float stepHeight = 3f;

        yield return StartCoroutine(FootUtil.lerpMove(tipTarget, targetPos, stepTime, stepHeight));

        //발 땅에 갖다 놓음
        tipTarget.transform.position = targetPos;
        isMoving = false;
    }

    public void SetTarget(Transform newTarget)
    {
        this.target = newTarget;

        if (target != null)

            tipTarget.position = target.position;
    }
}