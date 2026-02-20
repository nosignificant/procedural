using UnityEngine;

public class LegControl : MonoBehaviour
{
    [Header("References")]
    public Leg[] legs;
    public Transform movementTarget;

    private void Start()
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

        ApplyTargetToLegs(movementTarget);
    }

    public void SetMovementTarget(Transform newTarget)
    {
        if (newTarget == movementTarget) return;
        movementTarget = newTarget;
        ApplyTargetToLegs(movementTarget);
    }

    private void ApplyTargetToLegs(Transform target)
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

        if (legs == null) return;

        foreach (Leg leg in legs)
        {
            if (leg == null) continue;
            leg.SetTarget(target);
        }
    }

    public bool CheckValidFootPos(Vector3 destPos, Leg legAdding)
    {
        if (Vector3.Distance(destPos, legAdding.transform.position) < 0.1f)
            return false;

        if (legs != null)
        {
            foreach (Leg otherLeg in legs)
            {
                if (otherLeg == legAdding) continue;
                if (otherLeg.tipTarget == null) continue;

                float d = Vector3.Distance(otherLeg.tipTarget.position, destPos);
                if (d < legAdding.stride * 0.5f)
                    return false;
            }
        }
        return true;
    }
}