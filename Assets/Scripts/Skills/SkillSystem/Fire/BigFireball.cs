using UnityEngine;

public class BigFireball : Fireball
{
    [Header("Explosion")]
    [SerializeField] private float _explosionRadius = 6f;
    [SerializeField] private float _aoeMultiplier   = 2f;
    protected override void HitAndStop()
    {
        Explode();
        base.HitAndStop();
    }

    protected override void DestroyFireball()
    {
        Explode();
        base.DestroyFireball();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable tgt)) continue;

            float dmg = _instantDamage * _aoeMultiplier;
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref dmg, ref type);
            tgt.ReceiveDamage(dmg, type);
        }
    }
}