using UnityEngine;

namespace Game.Creatures
{
    [DisallowMultipleComponent]
    public sealed class WanderBehavior : MonoBehaviour
    {
        [Header("Wander Settings")]
        [Min(0.1f)] public float wanderRadius = 6f;
        [Min(0.1f)] public float arriveDistance = 1.2f;
        [Min(0.0f)] public float minRetargetTime = 0.5f;

        [Header("Walking (optional ground snap)")]
        public bool snapToGround = false;
        public LayerMask groundMask;
        [Min(0.0f)] public float groundRayHeight = 5f;
        [Min(0.1f)] public float groundRayDepth = 20f;

        [Header("Tick")]
        [Min(0.02f)] public float tickInterval = 0.2f;

        [Header("Debug")]
        public bool drawGizmos = true;

        public Transform CurrentTarget => targetProxy;

        private Transform targetProxy;
        private Vector3 wanderPoint;
        private bool hasPoint;
        private float retargetTimer;
        private float tickTimer;

        private void Awake()
        {
            EnsureProxy();
            SetPoint(transform.position);
        }

        private void EnsureProxy()
        {
            if (targetProxy != null) return;

            var go = new GameObject($"{name}_WanderTarget");
            go.transform.SetParent(transform, worldPositionStays: true);
            go.hideFlags = HideFlags.DontSave;
            targetProxy = go.transform;
            targetProxy.position = transform.position;
        }

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer < tickInterval) return;
            tickTimer = 0f;

            retargetTimer += tickInterval;

            if (!hasPoint || (HasArrived() && retargetTimer >= minRetargetTime))
            {
                if (TryPickPoint(out var p))
                    SetPoint(p);
            }
        }

        private bool HasArrived()
        {
            float d2 = (transform.position - wanderPoint).sqrMagnitude;
            return d2 <= arriveDistance * arriveDistance;
        }

        private void SetPoint(Vector3 p)
        {
            hasPoint = true;
            wanderPoint = p;
            retargetTimer = 0f;
            targetProxy.position = p;
        }

        private bool TryPickPoint(out Vector3 result)
        {
            Vector3 center = transform.position;
            float r = Mathf.Max(0.1f, wanderRadius);

            for (int i = 0; i < 12; i++)
            {
                Vector3 p = center + new Vector3(Random.Range(-r, r), 0f, Random.Range(-r, r));

                if (snapToGround)
                {
                    p.y = center.y;
                    if (!SnapToGround(ref p))
                        continue;
                }
                else
                {
                    p.y = center.y;
                }

                result = p;
                return true;
            }

            result = center;
            return false;
        }

        private bool SnapToGround(ref Vector3 p)
        {
            Vector3 origin = p + Vector3.up * groundRayHeight;
            if (Physics.Raycast(origin, Vector3.down, out var hit, groundRayDepth, groundMask))
            {
                p = hit.point;
                return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);

            if (targetProxy != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(targetProxy.position, 0.4f);
                Gizmos.DrawLine(transform.position, targetProxy.position);
            }
        }
    }
}