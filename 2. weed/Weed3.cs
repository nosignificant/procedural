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

    void LateUpdate()
    {
        if (line != null)
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

        // -------------------------------------------------------------
        // [수정 1] 유효성 검사 (찾은 위치가 원래 허공 위치랑 똑같으면?)
        // -------------------------------------------------------------
        // float 정밀도 오차를 고려해 아주 작은 거리차이로 비교
        if (Vector3.Distance(destPos, targetPos) < 0.01f)
        {
            // 땅을 못 찾았으므로 이동 취소
            isMoving = false;
            yield break;
        }

        // -------------------------------------------------------------
        // [수정 2] 노말 구하기 (인자 변경)
        // -------------------------------------------------------------
        // 이제 발(foot) 위치가 아니라 몸통(top) 위치를 기준으로 
        // "몸통에서 발 쪽으로 쏘았을 때의 바닥 각도"를 구합니다.
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
            Vector3 sagVector = surfaceNormal;

            Vector3 finalPos = lower.transform.position + (finalDir * offset);
            finalPos += sagVector * curve * 2.0f;

            current.position = Vector3.Lerp(current.position, finalPos, Time.deltaTime * 20f);

            current.LookAt(lower.transform, surfaceNormal);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        this.target = newTarget;
        if (target != null) tipTarget.position = target.position;
    }
}