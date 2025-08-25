using UnityEngine;

public class BigFireball : Fireball
{
    [Header("Explosion")]
    [SerializeField] private float _explosionRadius = 6f;
    [SerializeField] private float _aoeMultiplier   = 2f;
    private bool _exploded;

    protected override void HitAndStop()
    {
        Explode();
        base.HitAndStop();
    }
    
    private void OnDestroy()
    {
        if (!_exploded) Explode();
    }

    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable target)) continue;

            var ctx = new DamageContext
            {
                Attacker = _context,
                Target = target,
                SkillBehaviour = null,
                SkillDef = null,
                Slot = SkillSlot.Special,
                Type = SkillDamageType.Basic,
                Damage = _instantDamage * _aoeMultiplier,
                IsCrit = false,
                CritMultiplier = 1f,
                HitPoint = h.transform.position,
                HitNormal = Vector3.up,
                SourceGO = gameObject
            };

            _context.ApplyDamageContextModifiers(ref ctx);
            target.ReceiveDamage(ctx);
        }

    }
}