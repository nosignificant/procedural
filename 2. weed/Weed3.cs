using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weed3 : MonoBehaviour
{
    public LayerMask ground;

    [Header("Components")]
    public Transform foot;
    public Transform top;
    public Transform[] parts;
    public Transform tipTarget;
    public Transform target;

    [Header("Settings")]
    public float stride = 2f;
    private float offset;
    private float topToTargetDist;

    [Header("Moving")]
    private Vector3 movingDir;
    private bool isMoving = false;
    private Vector3 targetPos;
    private float maxBodyLength;

    [Header("Draw")]

    public LineRender line;
    private bool isDrawArrayInitialized = false;
    private Transform[] drawPoints;


    void Start()
    {
        maxBodyLength = stride * 1.1f;

        line = GetComponent<LineRender>();

        if (tipTarget != null)
        {
            Vector3 initPos = tipTarget.position;
            initPos = FootUtil.SetTargetNearest(initPos, ground);

            targetPos = initPos;
            tipTarget.position = targetPos;

            if (target != null) top.position = target.position;
        }
    }

    void Update()
    {

        MoveFootandClampedTop();

        bodyFABRIK();

        float currentBodyLen = Vector3.Distance(foot.position, top.position);

        // 현재 몸 길이가 보폭을 넘으면 발을 뗌
        if (!isMoving && currentBodyLen > stride)
        {
            StartCoroutine(MoveFoot());
        }
    }

    //선 그리기
    void LateUpdate()
    {
        if (line != null && isDrawArrayInitialized)
            line.Draw(drawPoints);

        else if (line != null)
            line.Draw(parts);

    }

    void MoveFootandClampedTop()
    {
        foot.position = Vector3.Lerp(foot.position, tipTarget.position, Time.deltaTime * 15f);

        Vector3 vectorToTarget = target.position - foot.position;
        Vector3 clampedVector = Vector3.ClampMagnitude(vectorToTarget, maxBodyLength);

        Vector3 constrainedTopPos = foot.position + clampedVector;

        top.position = Vector3.Lerp(top.position, constrainedTopPos, Time.deltaTime * 5f);
    }

    IEnumerator MoveFoot()
    {
        isMoving = true;

        Vector3 dirToTarget = (target.position - foot.position).normalized;
        Vector3 destPos = foot.position + (dirToTarget * stride);

        // 1. 가장 가까운 벽/땅 위치 찾기
        targetPos = FootUtil.SetTargetNearest(destPos, ground);

        // [수정 1] 유효성 검사 (찾은 위치가 원래 허공 위치랑 똑같으면?)
        if (Vector3.Distance(destPos, targetPos) < 0.01f)
        {
            // 땅을 못 찾았으므로 이동 취소
            isMoving = false;
            yield break;
        }
        // 노말 구하기 (인자 변경)
        Vector3 targetNormal = FootUtil.GetNormal(targetPos, top.position, ground);

        float stepTime = 0.4f;
        float stepHeight = 3f;

        yield return StartCoroutine(FootUtil.lerpMove(tipTarget, targetPos, stepTime, stepHeight, targetNormal));

        tipTarget.position = targetPos;

        yield return new WaitForSeconds(0.05f);

        isMoving = false;
    }

    void bodyFABRIK()
    {
        topToTargetDist = Vector3.Distance(top.position, tipTarget.position);
        offset = topToTargetDist / (parts.Length - 1);

        Vector3 surfaceNormal = FootUtil.GetNormal(top.position, tipTarget.position, ground);

        for (int i = parts.Length - 2; i >= 0; i--)
        {
            Transform current = parts[i];
            Transform lower = parts[i + 1];

            Vector3 footTotopDir = (top.position - tipTarget.position).normalized;
            Vector3 dir = (current.position - lower.position).normalized;
            Vector3 finalDir = Vector3.Lerp(dir, footTotopDir, 0.5f).normalized;

            float t = (float)i / (parts.Length - 1);
            float curve = Mathf.Sin(t * Mathf.PI);

            //노말 방향으로 굽힘
            //Vector3 sagVector = surfaceNormal;

            Vector3 finalPos = lower.transform.position + (finalDir * offset);
            //finalPos += sagVector * curve * 2.0f;

            current.position = Vector3.Lerp(current.position, finalPos, Time.deltaTime * 20f);

            current.LookAt(lower.transform, surfaceNormal);
        }
    }

    // Weed3.cs 내부의 SetTarget 함수
    public void SetTarget(Transform newTarget)
    {
        this.target = newTarget;
        if (target != null) tipTarget.position = target.position;

        // [핵심] Start()보다 SetTarget이 먼저 실행될 경우를 대비한 안전장치
        if (drawPoints == null && parts != null && parts.Length > 0)
        {
            drawPoints = new Transform[parts.Length + 1];
            // 몸통 파츠들을 1번 인덱스부터 미리 채워둠
            for (int i = 0; i < parts.Length; i++)
                drawPoints[i + 1] = parts[i];
        }

        // 0번 자리에 따라다닐 대상(inChild)을 연결
        if (drawPoints != null)
        {
            drawPoints[0] = target;
            isDrawArrayInitialized = true;
        }
    }
}