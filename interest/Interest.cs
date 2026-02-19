// File: creature/Interest.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Interest : MonoBehaviour
{
    [Header("References")]
    public CreatureScanner scanner;

    [Header("Preferences")]
    public Faction myFaction;
    public List<Faction> foodFactions = new List<Faction>();
    public List<Faction> enemyFactions = new List<Faction>();

    [Header("Decision Result")]
    public Transform currentTarget;
    public bool isFleeing;

    [Header("Weight Settings")]
    public float distanceWeight = 10f;

    // 로그 스팸 방지용 상태
    private bool hadAny;
    private int lastFriendCount;
    private int lastEnemyCount;
    private int lastFoodCount;

    private void Update()
    {
        if (scanner == null) return;

        ThinkAndLog();
        // TODO: 이동/행동은 여기서 currentTarget 기반으로 처리
    }

    private void ThinkAndLog()
    {
        IReadOnlyList<InterestTarget> detected = scanner.Results;

        int friendCount = 0;
        int enemyCount = 0;
        int foodCount = 0;

        if (detected == null || detected.Count == 0)
        {
            currentTarget = null;
            isFleeing = false;

            // 변화가 있을 때만 로그
            if (hadAny)
            {
                Debug.Log($"[{name}] 주변에 더 이상 대상이 없습니다.");
                hadAny = false;
                lastFriendCount = lastEnemyCount = lastFoodCount = 0;
            }
            return;
        }

        hadAny = true;

        InterestTarget best = null;
        float highestScore = float.MinValue;
        bool shouldFlee = false;

        for (int i = 0; i < detected.Count; i++)
        {
            var t = detected[i];
            if (t == null) continue;
            if (t.rootTransform == null) continue;

            // 분류 카운트
            bool isFood = foodFactions.Contains(t.faction);
            bool isEnemy = enemyFactions.Contains(t.faction);
            bool isFriend = (!isFood && !isEnemy && t.faction == myFaction);

            if (isFood) foodCount++;
            if (isEnemy) enemyCount++;
            if (isFriend) friendCount++;

            float score = CalculateScore(t, out bool isDanger);
            if (score > highestScore)
            {
                highestScore = score;
                best = t;
                shouldFlee = isDanger;
            }
        }

        // 상태 변화가 있을 때만 로그(원하는 문구)
        if (friendCount != lastFriendCount || enemyCount != lastEnemyCount || foodCount != lastFoodCount)
        {
            if (friendCount > 0) Debug.Log($"[{name}] 주변에 친구를 찾았습니다! (친구 {friendCount})");
            if (enemyCount > 0) Debug.Log($"[{name}] 주변에 적을 찾았습니다! (적 {enemyCount})");
            if (foodCount > 0) Debug.Log($"[{name}] 주변에 먹이를 찾았습니다! (먹이 {foodCount})");

            lastFriendCount = friendCount;
            lastEnemyCount = enemyCount;
            lastFoodCount = foodCount;
        }

        if (best != null)
        {
            currentTarget = best.rootTransform;
            isFleeing = shouldFlee;
        }
        else
        {
            currentTarget = null;
            isFleeing = false;
        }
    }

    private float CalculateScore(InterestTarget target, out bool isDanger)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        float distScore = (distanceWeight / (distance + 0.1f));

        isDanger = false;

        if (foodFactions.Contains(target.faction))
        {
            return target.weight + distScore;
        }

        if (enemyFactions.Contains(target.faction))
        {
            isDanger = true;
            return (target.weight + distScore) * 2.0f;
        }

        // 친구/중립은 약하게
        bool isFriend = (target.faction == myFaction);
        return distScore * (isFriend ? 0.2f : 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (currentTarget == null) return;

        Gizmos.color = isFleeing ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, currentTarget.position);
        Gizmos.DrawSphere(currentTarget.position + Vector3.up * 2, 0.5f);
    }
}