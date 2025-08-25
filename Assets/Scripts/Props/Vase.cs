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
    public bool CanBeDamaged { get; set; } = true;
    public event Action<Transform> OnEnemyDead;
    public void ReceiveDamage(in DamageContext ctx)
    {
        if (!CanBeDamaged) return;

        ctx.Attacker.Hp.ReceiveHP(_healAmount);

        if (_explosionParticles)
        {
            _explosionParticles.transform.SetParent(null);
            _explosionParticles.Play(true);
        }

        CanBeDamaged = false;
        Destroy(gameObject);
    }
}
