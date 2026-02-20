using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CreatureScanner : MonoBehaviour
{
    [Header("Settings")]
    public float scanRadius = 15f;
    public LayerMask targetLayer;
    public float scanInterval = 0.2f;

    [Header("Performance")]
    [Min(8)] public int maxHits = 64;

    //readonly: 한번 정해진 참조를 바꿀 수 없음 
    private readonly List<InterestTarget> nearby = new List<InterestTarget>(64);
    private Collider[] hitBuffer;

    //읽기 전용
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

        //반경에 들어온 개체 hitBuffer 리스트에 담음 
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
            if (col.transform.root == transform.root) continue;

            // 콜라이더가 자식에 붙어있을 수 있으니 InParent
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