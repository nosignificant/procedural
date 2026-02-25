// File: Assets/script/6.creatureTest/LegControl.cs
using UnityEngine;

public class LegControl : MonoBehaviour
{
    [Header("References")]
    public Leg[] legs;
    [SerializeField] private TargetControl targetControl;
    public KabschSpawner kabschSpawner;
    public bool useKabschCenter = false;

    private void Awake()
    {
        if (targetControl == null) targetControl = GetComponent<TargetControl>();
        if (kabschSpawner == null) kabschSpawner = GetComponent<KabschSpawner>();
    }
    void Update()
    {
        if (useKabschCenter) ApplyTargetToLegs(kabschSpawner.center);
    }

    private void OnEnable()
    {
        if (targetControl != null && !useKabschCenter)
            targetControl.TargetChanged += ApplyTargetToLegs;
    }

    private void OnDisable()
    {
        if (targetControl != null && !useKabschCenter)
            targetControl.TargetChanged -= ApplyTargetToLegs;
    }

    private void Start()
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

        if (targetControl != null)
            ApplyTargetToLegs(targetControl.movementTarget);
    }

    private void ApplyTargetToLegs(Transform target)
    {
        if (legs == null || legs.Length == 0)
            legs = GetComponentsInChildren<Leg>();

        if (legs == null) return;

        for (int i = 0; i < legs.Length; i++)
        {
            var leg = legs[i];
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
                if (d < legAdding.stride * 0.7f)
                    return false;
            }
        }
        return true;
    }
}