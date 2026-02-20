// File: Assets/script/interest/Interest.cs
using UnityEngine;
using Game.Creatures;
using System.Collections.Generic;

[DisallowMultipleComponent]
public sealed class Interest : MonoBehaviour
{
    [Header("References")]
    public CreatureScanner scanner;
    public Creature selfCreature;

    [Header("Decision Output")]
    [SerializeField] private CreatureIntent intent = CreatureIntent.Wander;
    [SerializeField] private Transform currentTarget;
    public Transform CurrentTarget => currentTarget;
    public CreatureIntent Intent => intent;

    [Header("EQS (Point Query)")]
    public bool useEqs = true;
    public float pointSpacing = 2f;
    public int maxPoints = 100;
    public LayerMask groundMask;
    public float groundRayHeight = 5f;
    public float groundRayDepth = 20f;

    public bool isWalkingCreature = false;
    public bool isFlyingCreature = false;

    [Header("Scoring")]
    public float foodAttract = 1.0f;
    public float enemyRepel = 2.0f;
    public float distanceFalloff = 1.0f;

    [Header("Optional LOS (wall blocking)")]
    public bool useLineOfSight = false;
    public LayerMask obstacleMask;

    [Header("Memory (Infinite until arrived)")]
    [Min(0f)] public float rememberStopDistance = 1.0f;     // 이 거리 안이면 도착
    [Min(0f)] public float rememberMinScoreToStore = 0.01f; // 이 점수 이상이면 기억 갱신

    [Header("Debug")]
    public bool drawEqsPoints = true;

    private Transform proxyTarget;

    private readonly List<Vector3> lastPoints = new();
    private readonly List<float> lastScores = new();

    // ✅ 무한 메모리
    private bool hasMemory;
    private Vector3 rememberedPoint;
    private float rememberedScore;

    private void Awake()
    {
        if (scanner == null) scanner = GetComponent<CreatureScanner>();
        if (selfCreature == null) selfCreature = GetComponent<Creature>();

        EnsureProxyTarget();
        currentTarget = proxyTarget;

        hasMemory = false;
        rememberedPoint = transform.position;
        rememberedScore = 0f;
    }

    //따라다닐 임시 타겟
    private void EnsureProxyTarget()
    {
        if (proxyTarget != null) return;

        var go = new GameObject($"{name}_EQS_Target");
        go.transform.SetParent(transform, worldPositionStays: true);
        go.hideFlags = HideFlags.DontSave;

        proxyTarget = go.transform;
        proxyTarget.position = transform.position;
    }

    private void Update()
    {
        if (scanner == null || selfCreature == null || selfCreature.Data == null || !useEqs)
        {
            intent = CreatureIntent.Wander;
            EnsureProxyTarget();
            proxyTarget.position = transform.position;
            currentTarget = proxyTarget;
            hasMemory = false;
            return;
        }

        EnsureProxyTarget();

        if (hasMemory)
        {   //메모리 있을 때 도착했는지 확인
            if (ArrivedAtMemory())
            {
                hasMemory = false;
                intent = CreatureIntent.Wander;
                proxyTarget.position = transform.position;
                currentTarget = proxyTarget;
                return;
            }

            // 메모리가 있으면 기본 타겟은 메모리
            proxyTarget.position = rememberedPoint;
            currentTarget = proxyTarget;
            intent = CreatureIntent.Chase;
        }

        // ✅ 그리고 “감지된 게 있으면” 메모리를 갱신(더 좋은 곳으로 업데이트)
        RunEqsAndMaybeUpdateMemory();
    }

    private bool ArrivedAtMemory()
    {
        return Vector3.Distance(transform.position, rememberedPoint) <= rememberStopDistance;
    }

    private void StoreMemory(Vector3 point, float score)
    {
        if (score < rememberMinScoreToStore) return;

        rememberedPoint = point;
        rememberedScore = score;
        hasMemory = true;
    }

    private void RunEqsAndMaybeUpdateMemory()
    {
        var detected = scanner.Results;

        // 감지된 게 없으면: 메모리가 있으면 유지(이미 Update에서 유지함), 없으면 Wander
        if (detected == null || detected.Count == 0)
        {
            if (!hasMemory)
            {
                intent = CreatureIntent.Wander;
                proxyTarget.position = transform.position;
                currentTarget = proxyTarget;
            }
            return;
        }

        BuildQueryPoints();

        Vector3 bestPoint = transform.position;
        float bestScore = float.NegativeInfinity;

        for (int i = 0; i < lastPoints.Count; i++)
        {
            Vector3 p = lastPoints[i];
            float score = ScorePoint(p, detected);
            if (i < lastScores.Count) lastScores[i] = score;

            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = p;
            }
        }

        // “좋은 점수”면 메모리 갱신
        if (bestScore > rememberMinScoreToStore)
            StoreMemory(bestPoint, bestScore);

        // 메모리가 없었고 이번에 처음 생겼으면 즉시 타겟으로 반영
        if (hasMemory)
        {
            proxyTarget.position = rememberedPoint;
            currentTarget = proxyTarget;
            intent = CreatureIntent.Chase;
        }
        else
        {
            // 점수가 별로면 배회 유지
            intent = CreatureIntent.Wander;
            proxyTarget.position = transform.position;
            currentTarget = proxyTarget;
        }
    }

    private void BuildQueryPoints()
    {
        lastPoints.Clear();
        lastScores.Clear();

        float r = Mathf.Max(0.1f, scanner.scanRadius * 2);
        float s = Mathf.Max(0.25f, pointSpacing);
        Vector3 center = transform.position;

        if (isFlyingCreature)
        {
            for (int i = 0; i < maxPoints; i++)
            {
                Vector3 p = center + Random.insideUnitSphere * r;
                lastPoints.Add(p);
                lastScores.Add(0f);
            }
            return;
        }

        int steps = Mathf.CeilToInt((r * 2f) / s);
        for (int x = -steps; x <= steps; x++)
        {
            for (int z = -steps; z <= steps; z++)
            {
                if (lastPoints.Count >= maxPoints) return;

                Vector3 p = center + new Vector3(x * s, 0f, z * s);
                if ((p - center).sqrMagnitude > r * r) continue;

                p.y = center.y;
                if (!SnapToGround(ref p)) continue;

                lastPoints.Add(p);
                lastScores.Add(0f);
            }
        }
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

    private float ScorePoint(Vector3 point, IReadOnlyList<InterestTarget> detected)
    {
        float total = 0f;

        for (int i = 0; i < detected.Count; i++)
        {
            var t = detected[i];
            if (t == null || t.rootTransform == null) continue;
            if (t.SpeciesId == 0) continue;

            RelationType rel = selfCreature.Data.GetRelationTo(t.SpeciesId);

            if (useLineOfSight && IsBlocked(point, t.rootTransform.position))
                continue;

            float d = Vector3.Distance(point, t.rootTransform.position);
            float w = Mathf.Max(0f, t.Weight);
            float distTerm = 1f / Mathf.Pow(d + 1f, distanceFalloff);

            if (rel == RelationType.Food) total += (foodAttract * w) * distTerm;
            else if (rel == RelationType.Enemy) total -= (enemyRepel * w) * distTerm;
        }

        return total;
    }

    private bool IsBlocked(Vector3 from, Vector3 to)
    {
        Vector3 a = from + Vector3.up * 1.0f;
        Vector3 b = to + Vector3.up * 1.0f;
        Vector3 dir = b - a;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return false;
        dir /= dist;
        return Physics.Raycast(a, dir, dist, obstacleMask);
    }

    private void OnDrawGizmos()
    {
        if (proxyTarget == null) return;

        Gizmos.color = (intent == CreatureIntent.Flee) ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, proxyTarget.position);
        Gizmos.DrawSphere(proxyTarget.position + Vector3.up * 2, 0.5f);

        if (hasMemory)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rememberedPoint + Vector3.up * 1.0f, 0.4f);
        }
    }
}