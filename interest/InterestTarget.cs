// File: creature/InterestTarget.cs
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InterestTarget : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Creature creature;

    [Header("Override")]
    [SerializeField] private bool useManualValues;

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

        if (!useManualValues && creature != null && creature.Data != null)
        {
            faction = creature.Data.faction;
            weight = creature.Data.interestWeight;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (!useManualValues && creature != null && creature.Data != null)
        {
            faction = creature.Data.faction;
            weight = creature.Data.interestWeight;
        }
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