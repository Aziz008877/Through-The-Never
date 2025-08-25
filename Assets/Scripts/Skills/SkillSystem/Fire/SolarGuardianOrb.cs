using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SolarGuardianOrb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _idleVfx;
    [SerializeField] private Fireball _projectilePrefab;
    private float _damage;
    private float _interval;
    private float _radius;
    private ActorContext _ctx;
    public void Init(float dmg, float interval, float radius, float life, ActorContext ctx)
    {
        _damage = dmg;
        _interval = interval;
        _radius = radius;
        _ctx = ctx;

        if (_idleVfx) _idleVfx.Play();
        StartCoroutine(WorkRoutine());
        Destroy(gameObject, life);
    }
    private IEnumerator WorkRoutine()
    {
        var wait = new WaitForSeconds(_interval);

        while (true)
        {
            IDamageable tgt = FindClosest();
            if (tgt != null)
            {
                Vector3 dir = ((MonoBehaviour)tgt).transform.position - transform.position;
                Fire(dir.normalized, tgt);
            }
            yield return wait;
        }
    }
    
    private IDamageable FindClosest()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        IDamageable best = null;
        float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable d)) continue;
            float sqr = (h.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr) { best = d; bestSqr = sqr; }
        }
        return best;
    }

    private void Fire(Vector3 dir, IDamageable target)
    {
        var proj = Instantiate(_projectilePrefab, transform.position + dir * 0.6f, Quaternion.LookRotation(dir));
        proj.SetHoming(false);
        proj.EnableSmallExplosion(false);

        var ctx = new DamageContext
        {
            Attacker       = _ctx,
            Target         = null,                // цель определит коллайдер при столкновении
            SkillBehaviour = null,
            SkillDef       = null,
            Slot           = SkillSlot.Undefined,
            Type           = SkillDamageType.Basic,
            Damage         = _damage,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = Vector3.zero,
            HitNormal      = Vector3.up,
            SourceGO       = proj.gameObject
        };

        _ctx.ApplyDamageContextModifiers(ref ctx);
        proj.Init(ctx, 2f); // урон прилетит при попадании проектайла
    }
}