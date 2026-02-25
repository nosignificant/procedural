// File: Assets/script/targeting/TargetControl.cs
using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TargetControl : MonoBehaviour
{
    [SerializeField] public Transform movementTarget;

    public event Action<Transform> TargetChanged;

    public void SetMovementTarget(Transform newTarget)
    {
        if (ReferenceEquals(newTarget, movementTarget)) return;

        movementTarget = newTarget;
        TargetChanged?.Invoke(movementTarget);
    }
}