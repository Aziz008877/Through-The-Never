using System;
using UnityEngine;

public class UrnTrap : MonoBehaviour, IDamageable
{
    [SerializeField] private float _radius = 4f;

    public float CurrentHP { get; set; }
    public float MinHP { get; set; }
    public float MaxHP { get; set; }
    public bool CanBeDamaged { get; set; } = true;

    public event Action<Transform> OnEnemyDead;

    private static readonly Collider[] _hits = new Collider[64];
    private bool _exploded;
    private ActorContext _selfActor;

    private void Awake()
    {
        _selfActor = GetComponentInParent<ActorContext>();
    }

    public void ReceiveDamage(in DamageContext ctx)
    {
        if (!CanBeDamaged || _exploded) return;
        _exploded = true;
        CanBeDamaged = false;

        int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, _hits);
        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (!col) continue;

            if (col.TryGetComponent(out IDamageable dmg) && !ReferenceEquals(dmg, this))
                dmg.ReceiveDamage(in ctx);

            if (col.TryGetComponent(out ActorContext actor) && !ReferenceEquals(actor, _selfActor))
                actor.Hp.ReceiveDamage(5, null);
        }

        OnEnemyDead?.Invoke(transform);
        Destroy(gameObject);
    }
}