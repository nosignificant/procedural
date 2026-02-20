// File: creature/InterestTarget.cs
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InterestTarget : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Creature creature;

    [Header("References")]
    [SerializeField] public Transform rootTransform;

    public Creature Creature => creature;

    public int SpeciesId => (creature != null) ? creature.CreatureId : 0;

    public float Weight
    {
        get
        {
            if (creature == null || creature.Data == null) return 0f;
            return Mathf.Max(0f, creature.Data.interestWeight);
        }
    }

    public Transform RootTransform => rootTransform != null ? rootTransform : transform.root;

    private void Awake()
    {
        if (rootTransform == null) rootTransform = transform.root;

        if (creature == null)
            creature = GetComponentInParent<Creature>();
    }

    private void OnValidate()
    {
        if (rootTransform == null) rootTransform = transform.root;

        if (creature == null)
            creature = GetComponentInParent<Creature>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (creature != null && creature.Data != null) ? Color.cyan : Color.gray;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}