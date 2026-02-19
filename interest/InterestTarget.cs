// File: creature/InterestTarget.cs
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InterestTarget : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Creature creature;

    [Header("Runtime Tags")]
    public Faction faction;
    public float weight = 10f;

    [Header("References")]
    public Transform rootTransform;

    public Creature Creature => creature;
    public int CreatureId => creature != null ? creature.CreatureId : 0;

    private void Awake()
    {
        if (rootTransform == null) rootTransform = transform.root;

        if (creature == null)
        {
            // 보통 InterestTarget은 콜라이더에 붙고, Creature는 루트에 붙는 경우가 많아서.
            creature = GetComponentInParent<Creature>();
        }

        // 필요하면 여기서 creature.Data 기반으로 faction/weight 등을 초기화할 수 있음.
        // 지금 단계는 “탐지 + 분류 + 로그”가 목적이므로 최소만 유지.
    }

    private void OnDrawGizmos()
    {
        switch (faction)
        {
            case Faction.Plant: Gizmos.color = Color.green; break;
            case Faction.Carnivore: Gizmos.color = Color.red; break;
            case Faction.Herbivore: Gizmos.color = Color.yellow; break;
            default: Gizmos.color = Color.white; break;
        }

        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}