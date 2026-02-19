using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Creature : MonoBehaviour
{
    [Header("Data (Species Shared)")]
    [SerializeField] private CreatureData creatureData;

    [Header("Runtime (Per-Instance)")]
    [SerializeField] private int currentHP;

    public CreatureData Data => creatureData;
    public int CreatureId => creatureData != null ? creatureData.creatureID : 0;
    public int MaxHP => creatureData != null ? creatureData.maxHP : 0;
    public int CurrentHP => currentHP;
    public bool IsDead => currentHP <= 0;

    public event Action<Creature> Died;
    public event Action<Creature, int, int> HpChanged; // (self, old, now)

    private void Awake()
    {
        if (creatureData == null)
            throw new InvalidOperationException($"{name}: CreatureData is not assigned.");

        ApplyIdentity();
        Respawn(fullHeal: true);
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && creatureData != null)
            ApplyIdentity();
    }

    private void ApplyIdentity()
    {
        if (!string.IsNullOrWhiteSpace(creatureData.creatureName))
            gameObject.name = creatureData.creatureName;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead) return;

        SetHP(currentHP - amount);

        if (currentHP == 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;
        SetHP(currentHP + amount);
    }

    public void Respawn(bool fullHeal)
    {
        gameObject.SetActive(true);

        if (fullHeal) SetHP(MaxHP);
        else SetHP(Mathf.Clamp(currentHP, 1, MaxHP));
    }

    public void Revive(int hp)
    {
        gameObject.SetActive(true);
        SetHP(Mathf.Clamp(hp, 1, MaxHP));
    }

    private void SetHP(int newHp)
    {
        int old = currentHP;
        currentHP = Mathf.Clamp(newHp, 0, MaxHP);
        if (old != currentHP)
            HpChanged?.Invoke(this, old, currentHP);
    }

    private void Die()
    {
        if (!gameObject.activeSelf) return;

        Died?.Invoke(this);

        // 기본: 죽으면 비활성화. 리스폰은 Respawn/Revive로.
        gameObject.SetActive(false);
    }
}