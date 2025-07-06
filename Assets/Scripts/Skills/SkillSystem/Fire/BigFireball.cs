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

            float damage = _instantDamage * _aoeMultiplier;
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref damage, ref type);
            target.ReceiveDamage(damage, type);
        }
    }
}