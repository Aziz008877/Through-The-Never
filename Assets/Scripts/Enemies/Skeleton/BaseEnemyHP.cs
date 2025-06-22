using System;
using UnityEngine;
public class BaseEnemyHP : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
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
        }
    }

    private void Die()
    {
        OnEnemyDead?.Invoke(transform);
        Destroy(gameObject);
    }
}
