
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weed2 : MonoBehaviour
{
    // 2. 타겟은 저번에 레이캐스트 했을 때처럼 발 근처 ... 아무튼 레이캐스트로 구함 
    // 타겟까지 거리가 일정이하면 붙음 
    // 3. 이 사이 구간을 각 파트가 일정한 간격으로 채움 
    // 4. 시작 - 끝 길이가 일정 이상을 넘어가면 타겟 - 시작지점 각도 유지한채로 고정 풀고 초기 길이로 돌아감 
    // 5. 거리가 일정 이하면 forward만해서 땅에 딱붙어있고 거 당김 리가 일정 이상이면 backward도 해서 다시 뿌리쪽으로
    // 뿌리쪽이 0 - 머리쪽이 5 

    public Weedpart2[] parts;
    public Weedpart2 head; // 머리가 땅에 붙음 
    public Weedpart2 root; // 뿌리가 몸에 붙음 


    public float defaultWeedLength;
    private Vector3 defaultRootPos;
    public float stickDist = 5.0f;
    private float offset;

    public GameObject targetPosPrefab;

    public Transform parent;

    private GameObject targetPosInstance;

    private Vector3 targetPos;
    private float rootToTargetDist;
    private Vector3 HeadRootDir;

    private bool isMoving = false;

    void Start()
    {
        defaultRootPos = parent.InverseTransformPoint(root.transform.position);
        if (targetPosPrefab != null)
        {
            targetPosInstance = Instantiate(targetPosPrefab, parent != null ? parent : transform);
        }
        targetPos = head.SetTargetGround();
    }

    void Update()
    {
        if (parent != null)
        {
            root.transform.position = parent.TransformPoint(defaultRootPos);
        }
        ForwardTarget();
        //타겟을 구했으니 타겟으로 옮길지 말지를 정해야함 
        //머리 - 뿌리 거리, 방향
        Vector3 diff = targetPos - root.transform.position;
        rootToTargetDist = Vector3.Distance(root.transform.position, targetPos);
        HeadRootDir = root.transform.position - head.transform.position;

        //x축이나 z축이 일정 거리 이상 멀어지면 새 타겟을 구한다.
        if (Mathf.Abs(diff.x) > 5 || Mathf.Abs(diff.z) > 5)
        {
            if (!isMoving)
                StartCoroutine(MoveFoot());
        }
    }

    // 머리(땅에 닿을 부분) 의 위치 - 타겟 구함
    // 타겟까지 거리가 일정 이하면 우선 머리부터 땅에 닿고 나머지 파트는 그 방향으로 따라감
    // 뿌리부분은 고정되어서 움직이지 않음 
    void ForwardTarget()
    {
        head.transform.position = Vector3.Lerp(head.transform.position, targetPos, Time.deltaTime); offset = rootToTargetDist / (parts.Length - 1);

        for (int i = parts.Length - 2; i > 0; i--)
        {
            Weedpart2 current = parts[i];
            Weedpart2 lower = parts[i + 1]; // 5번인 머리부터 

            Vector3 dir = (current.transform.position - lower.transform.position).normalized;

            Vector3 finalDir = Vector3.Lerp(dir, HeadRootDir, 0.5f).normalized;

            Vector3 finalPos = lower.transform.position + finalDir * offset;
            current.transform.position = Vector3.Lerp(current.transform.position, finalPos, Time.deltaTime);
            current.transform.LookAt(lower.transform);
        }
    }

    IEnumerator MoveFoot()
    {
        isMoving = true;
        targetPos = head.SetTargetGround();

        // 타겟 - 나 해야 양수 방향 나옴 
        Vector3 dir = targetPos - head.transform.position;
        Vector3 startPos = head.transform.position;
        float t = 0f;
        float stepTime = 2f;
        float stepHeight = 4f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / stepTime;
            //lerp가 뭐지 
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            float heightCurve = Mathf.Sin(t * Mathf.PI) * stepHeight;

            head.transform.position = currentPos + Vector3.up * heightCurve;
            targetPosInstance.transform.position = currentPos + Vector3.up * heightCurve;


            yield return null;
        }
        head.transform.position = targetPos;
        targetPosInstance.transform.position = targetPos;
        isMoving = false;

        yield return null;

    }
}