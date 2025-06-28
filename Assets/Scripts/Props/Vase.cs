using System;
using UnityEngine;

public class Vase : MonoBehaviour, IDamageable
{
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private PlayerHP _playerHp;
    [SerializeField] private float _healAmount;
    public float CurrentHP { get; set; }
    public float MinHP { get; set; }
    public float MaxHP { get; set; }
    public Action<Transform> OnEnemyDead { get; set; }
    public void ReceiveDamage(float damageValue, SkillDamageType type)
    {
        _playerHp.ReceiveHP(_healAmount);
        _explosionParticles.transform.SetParent(null);
        _explosionParticles.Play();
        Destroy(gameObject);
    }
}
