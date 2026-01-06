
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Weed2 : MonoBehaviour
{
    // 1. 뿌리는 땅에 박혀 있음
    // 2. 타겟은 저번에 레이캐스트 했을 때처럼 발 근처 ... 아무튼 레이캐스트로 구함 
    // 타겟까지 거리가 일정이하면 붙음 
    // 3. 이 사이 구간을 각 파트가 일정한 간격으로 채움 
    // 4. 시작 - 끝 길이가 일정 이상을 넘어가면 타겟 - 시작지점 각도 유지한채로 고정 풀고 초기 길이로 돌아감 
    // 5. 거리가 일정 이하면 forward만해서 땅에 딱붙어있고 거 당김 리가 일정 이상이면 backward도 해서 다시 뿌리쪽으로
    // 뿌리쪽이 0 - 머리쪽이 5 

    public Weedpart2[] parts;
    public Weedpart2 head; // 머리가 땅에 붙음 
    public Weedpart2 root; // 뿌리가 몸에 붙음 


    public float partsOffset = 1f;

    public float defaultWeedLength;
    private Vector3 defaultRootPos;
    public float stickDist = 5.0f;
    private float offset;

    public GameObject targetPosPrefab;

    public Transform parent;

    private GameObject targetPosInstance;


    void Start()
    {
        defaultRootPos = root.transform.position;
        if (targetPosPrefab != null)
        {
            targetPosInstance = Instantiate(targetPosPrefab, parent != null ? parent : transform);
        }
    }

    void Update()
    {
        MoveLegs();
    }

    void MoveLegs()
    {
        Vector3 targetPos = head.SetTargetGround();
        targetPosInstance.transform.position = targetPos;


        ForwardTarget(targetPos);
    }

    // 머리(땅에 닿을 부분) 의 위치 - 타겟 구함
    //타겟까지 거리가 일정 이하면 우선 머리부터 땅에 닿고 나머지 파트는 그 방향으로 따라감
    // 뿌리부분은 고정되어서 움직이지 않음 
    void ForwardTarget(Vector3 targetPos)
    {
        head.transform.position = targetPos;

        //일단 머리부분을 땅에 붙이고 거리를 잰다 
        float headToRootDist = Vector3.Distance(head.transform.position, root.transform.position);

        //머리 ~ 땅 / 각 파트 = 각 오프셋
        offset = headToRootDist / (parts.Length - 3);

        Vector3 HeadRootDir = (root.transform.position - head.transform.position).normalized;
        // 머리부터 , 5번은 머리라 움직이지는 않음
        //  0번인 뿌리도 제외
        for (int i = parts.Length - 2; i > 0; i--)
        {
            Weedpart2 current = parts[i];
            Weedpart2 lower = parts[i + 1]; // 5번인 머리부터 

            Vector3 dir = (current.transform.position - lower.transform.position).normalized;

            Vector3 finalDir = Vector3.Lerp(dir, HeadRootDir, 0.5f).normalized;

            current.transform.position = lower.transform.position + finalDir * offset;
            current.transform.LookAt(head.transform);
        }
        root.transform.position = defaultRootPos;
    }
    void BackwardTarget(Vector3 targetPos)
    {
        root.transform.position = defaultRootPos;

        for (int i = 1; i < parts.Length - 1; i++)
        {
            Weedpart2 current = parts[i];
            Weedpart2 upper = parts[i - 1]; // 뿌리

            Vector3 dir = (current.transform.position - upper.transform.position).normalized;

            current.transform.position = upper.transform.position + dir * offset;
            current.transform.LookAt(head.transform);
        }
    }
}