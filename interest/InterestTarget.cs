// File: creature/InterestTarget.cs
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InterestTarget : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Creature creature;

    [Header("Override (for non-creatures like Weed)")]
    [Tooltip("true면 아래 manual 값을 사용. false면 CreatureData에서 자동 세팅.")]
    [SerializeField] private bool useManualValues = false;

    [Tooltip("Creature가 없거나 강제 지정할 때 사용. 0이면 Creature.CreatureId 사용.")]
    [SerializeField] private int speciesIdOverride = 0;

    [Tooltip("점수 계산에 쓰이는 대상 가중치. (보통 CreatureData.interestWeight)")]
    [SerializeField] private float weight = 10f;

    [Header("References")]
    public Transform rootTransform;

    public Creature Creature => creature;

    /// <summary>
    /// 관계 판정을 위한 종 ID.
    /// - manual이면 speciesIdOverride 우선
    /// - 아니면 Creature.CreatureId
    /// </summary>
    public int SpeciesId
    {
        get
        {
            if (useManualValues && speciesIdOverride != 0) return speciesIdOverride;
            return creature != null ? creature.CreatureId : speciesIdOverride;
        }
    }

    /// <summary>
    /// 점수 계산용 가중치(대상 가치).
    /// </summary>
    public float Weight => weight;

    private void Awake()
    {
        if (rootTransform == null) rootTransform = transform.root;

        if (creature == null)
            creature = GetComponentInParent<Creature>();

        ApplyFromCreatureDataIfNeeded();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (rootTransform == null) rootTransform = transform.root;
        if (creature == null) creature = GetComponentInParent<Creature>();

        ApplyFromCreatureDataIfNeeded();
    }

    private void ApplyFromCreatureDataIfNeeded()
    {
        if (useManualValues) return;
        if (creature == null || creature.Data == null) return;

        // 종 데이터 기반 자동 세팅
        weight = creature.Data.interestWeight;
        // speciesIdOverride는 보통 건드리지 않음(자동일 때는 CreatureId를 쓰니까)
    }

    private void OnDrawGizmos()
    {
        // 디버그용: 수동이면 마젠타, 자동(데이터 링크)이면 시안, 미연결이면 회색
        if (useManualValues) Gizmos.color = Color.magenta;
        else if (creature != null && creature.Data != null) Gizmos.color = Color.cyan;
        else Gizmos.color = Color.gray;

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}