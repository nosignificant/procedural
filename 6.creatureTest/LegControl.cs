using UnityEngine;
using System.Linq;

public class LegControl : MonoBehaviour
{
    public Leg[] legs;
    public Interest interest;
    public Transform target;
    Transform lastTarget;


    void Start()
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

        interest = GetComponent<Interest>();
        if (target == null) target = transform;
        lastTarget = target;
    }

    private void Update()
    {
        if (interest == null) { Debug.Log("interest 설정을 해주세요"); return; }

        target = interest.CurrentTarget;

        if (target == lastTarget) return;

        lastTarget = target;
        ApplyTargetToLegs(target);
    }

    private void ApplyTargetToLegs(Transform t)
    {
        if (legs == null) return;

        foreach (var leg in legs)
        { leg.SetTarget(t); }
    }
    public bool CheckValidFootPos(Vector3 destPos, Leg legAdding)
    {
        //목표가 현재 위치랑 너무 가까우면 return
        if (Vector3.Distance(destPos, legAdding.transform.position) < 0.1f)
        {
            return false;
        }

        if (legs != null)
        {
            foreach (Leg otherLeg in legs)
            {
                if (otherLeg == legAdding) continue;
                float d = Vector3.Distance(otherLeg.tipTarget.position, destPos);
                if (d < legAdding.stride * 0.5)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
