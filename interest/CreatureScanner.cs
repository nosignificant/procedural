// File: creature/CreatureScanner.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CreatureScanner : MonoBehaviour
{
    [Header("Settings")]
    public float scanRadius = 15f;
    public LayerMask targetLayer;
    public float scanInterval = 0.2f;

    [Header("Performance")]
    [Min(8)] public int maxHits = 64;

    private readonly List<InterestTarget> nearby = new List<InterestTarget>(64);
    private Collider[] hitBuffer;

    public IReadOnlyList<InterestTarget> Results => nearby;

    private void Awake()
    {
        hitBuffer = new Collider[Mathf.Max(8, maxHits)];
    }

    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    private System.Collections.IEnumerator ScanRoutine()
    {
        var wait = new WaitForSeconds(scanInterval);
        while (true)
        {
            PerformScan();
            yield return wait;
        }
    }

    private void PerformScan()
    {
        nearby.Clear();

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            scanRadius,
            hitBuffer,
            targetLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            var col = hitBuffer[i];
            if (col == null) continue;

            // "나는 제외"
            if (col.transform == transform) continue;

            // 콜라이더가 자식에 붙어있을 수 있으니 InParent 권장
            var target = col.GetComponentInParent<InterestTarget>();
            if (target == null) continue;

            // 중복 방지(한 대상이 콜라이더 여러 개일 수 있음)
            if (!nearby.Contains(target))
                nearby.Add(target);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}