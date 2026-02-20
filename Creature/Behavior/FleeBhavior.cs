using UnityEngine;

namespace Game.Creatures
{
    [DisallowMultipleComponent]
    public sealed class FleeBehavior : MonoBehaviour
    {
        [Header("Flee Settings")]
        [Min(0.1f)] public float fleeDistance = 8f;

        [Header("Tick")]
        [Min(0.02f)] public float tickInterval = 0.2f;

        [Header("Debug")]
        public bool drawGizmos = true;

        public Transform CurrentTarget => targetProxy;

        private Transform targetProxy;
        private float tickTimer;

        private void Awake()
        {
            EnsureProxy();
            targetProxy.position = transform.position;
        }

        private void EnsureProxy()
        {
            if (targetProxy != null) return;

            var go = new GameObject($"{name}_FleeTarget");
            go.transform.SetParent(transform, worldPositionStays: true);
            go.hideFlags = HideFlags.DontSave;
            targetProxy = go.transform;
            targetProxy.position = transform.position;
        }

        /// <summary>
        /// 추후: “도망칠 대상(enemy)”를 넘겨받으면 그 반대 방향으로 포인트를 세팅하는 방식으로 확장.
        /// </summary>
        public void SetFleeFrom(Vector3 dangerPosition)
        {
            Vector3 away = (transform.position - dangerPosition);
            away.y = 0f;

            if (away.sqrMagnitude < 0.001f)
                away = Random.insideUnitSphere;

            away.y = 0f;
            away.Normalize();

            targetProxy.position = transform.position + away * fleeDistance;
        }

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer < tickInterval) return;
            tickTimer = 0f;
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            if (targetProxy == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetProxy.position, 0.4f);
            Gizmos.DrawLine(transform.position, targetProxy.position);
        }
    }
}