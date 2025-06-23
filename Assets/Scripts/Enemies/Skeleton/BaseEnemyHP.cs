using System;
using UnityEngine;
using Zenject;

public class BaseEnemyHP : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    [Inject] private DamageTextPool _damageTextPool;
    public Action<Transform> OnEnemyDead { get; set; }

    public void ReceiveDamage(float damageValue, SkillDamageType type)
    {
        if (CurrentHP - damageValue <= MinHP)
        {
            Die();
        }
        else
        {
            CurrentHP -= damageValue;
            _damageTextPool.ShowDamage(damageValue, transform.position);
        }
    }

    private void Die()
    {
        OnEnemyDead?.Invoke(transform);
        Destroy(gameObject);
    }
}
