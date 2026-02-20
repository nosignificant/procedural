using UnityEngine;

namespace Game.Creatures
{
    [DisallowMultipleComponent]
    public sealed class CreatureBrain : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Interest interest;
        [SerializeField] private LegControl legControl;
        [SerializeField] private WanderBehavior wanderBehavior;
        [SerializeField] private FleeBehavior fleeBehavior;

        private void Awake()
        {
            if (interest == null) interest = GetComponent<Interest>();
            if (legControl == null) legControl = GetComponent<LegControl>();
            if (wanderBehavior == null) wanderBehavior = GetComponent<WanderBehavior>();
            if (fleeBehavior == null) fleeBehavior = GetComponent<FleeBehavior>();
        }

        private void Update()
        {
            if (legControl == null)
                return;

            Transform target = ResolveMovementTarget();
            legControl.SetMovementTarget(target);
        }

        private Transform ResolveMovementTarget()
        {
            if (interest == null)
                return wanderBehavior != null ? wanderBehavior.CurrentTarget : transform;

            switch (interest.Intent)
            {
                case CreatureIntent.Chase:
                    return interest.CurrentTarget != null
                        ? interest.CurrentTarget
                        : (wanderBehavior != null ? wanderBehavior.CurrentTarget : transform);

                case CreatureIntent.Flee:
                    return (fleeBehavior != null && fleeBehavior.CurrentTarget != null)
                        ? fleeBehavior.CurrentTarget
                        : (wanderBehavior != null ? wanderBehavior.CurrentTarget : transform);

                default: // ✅ Wander
                    return wanderBehavior != null ? wanderBehavior.CurrentTarget : transform;
            }
        }
    }
}