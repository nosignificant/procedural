using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Creature : MonoBehaviour
{
    //종족 공유 데이터 받아옴 
    [Header("Species Shared")]
    [SerializeField] private CreatureData creatureData;

    [Header("instance")]
    [SerializeField] private int currentHP;

    //getter 함수 c# 버전
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
        currentHP = MaxHP;
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

    private void SetHP(int newHp)
    {
        int old = currentHP;
        currentHP = Mathf.Clamp(newHp, 0, MaxHP);
        if (old != currentHP) // HpChange 구독한 함수에게 알림 나중에 UI만들때 참고
        { HpChanged?.Invoke(this, old, currentHP); }
    }

    private void Die()
    {
        if (!gameObject.activeSelf) return;

        Died?.Invoke(this);

        // 기본: 죽으면 비활성화. 리스폰은 Respawn/Revive로.
        gameObject.SetActive(false);
    }
}