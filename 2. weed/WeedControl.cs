
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeedControl : MonoBehaviour
{
    // 1. 뿌리는 땅에 박혀 있음
    // 2. 제일 윗부분은 타겟을 향함
    // 3. 타겟에게 붙을 때까지 원으로 회전
    // 4. 아래 부분은 연쇄적으로 위의 움직임을 따라함
    // 5. 타겟이 닿을 만큼 충분히 가까이 오면 제일 윗부부은 타겟에게 붙음
    // 6. 일정 시간 동안 특정 위치에 붙고 떨어지면 다른 랜덤한 위치에 붙음, 더듬는 거처럼 

    public Weedpart[] parts;
    public Weedpart weedHead;
    public Transform target;
    public Weedpart root;


    public float partsOffset = 1f;

    private float dist;
    private Transform weedHeadDefaultPos;
    private Transform rootDefaultPos;

    private Coroutine moveCoroutine;

    void Start()
    {
        rootDefaultPos = root.transform;
        weedHeadDefaultPos = weedHead.transform;
    }

    void Update()
    {
        dist = Util.fromThis2Target(transform.position, target.transform.position);
        if (dist > 5f) Follow();
        else Wander();

    }

    void Follow()
    {
        weedHead.Rotate(root.transform, 1f);
        weedHead.FollowTarget(target.transform.position);
        MoveBodyParts();
    }

    void Wander()
    {
        weedHead.Rotate(root.transform, 1f);
        weedHead.FollowTarget(GetWanderTarget());
        MoveBodyParts();
    }

    Vector3 GetWanderTarget()
    {
        float rootDist = Vector2.Distance(
                new Vector2(weedHead.transform.position.x, weedHead.transform.position.z),
                new Vector2(root.transform.position.x, root.transform.position.z));

        float defaultDist = Vector2.Distance(
                new Vector2(weedHeadDefaultPos.position.x, weedHeadDefaultPos.position.z),
                new Vector2(root.transform.position.x, root.transform.position.z));


        if (rootDist > defaultDist * 3f)
            return weedHeadDefaultPos.position;

        else return target.transform.position;
    }
    void MoveBodyParts()
    {
        Vector3 targetPos = weedHead.transform.position - (weedHead.transform.forward * partsOffset);
        parts[0].FollowTarget(targetPos);

        for (int i = 1; i < parts.Length; i++)
        {
            Weedpart previousPart = parts[i - 1];
            targetPos = previousPart.transform.position -
                    (previousPart.transform.forward * partsOffset);
            parts[i].FollowTarget(targetPos);
        }
        root.transform.position = rootDefaultPos.position;

        for (int i = parts.Length - 2; i > 0; i--)
        {
            Weedpart previousPart = parts[i + 1];
            Vector3 dir = (previousPart.transform.position - parts[i].transform.position).normalized;

            float dist = (float)i / parts.Length - 1;
            dist = dist * dist * partsOffset;

            targetPos = dir * dist;
            parts[i].FollowTarget(targetPos);
        }
        root.transform.position = rootDefaultPos.position;

    }

}