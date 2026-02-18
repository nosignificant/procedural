using UnityEngine;
using System.Linq;

public class LegControl : MonoBehaviour
{
    public Leg[] legs;

    void Start()
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

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
