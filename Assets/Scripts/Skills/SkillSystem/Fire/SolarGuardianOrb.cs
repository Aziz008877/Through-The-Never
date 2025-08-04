/*────────────────────────────  SolarGuardianOrb  ─────────────────────────*/

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SolarGuardianOrb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _idleVfx;
    [SerializeField] private Fireball       _projectilePrefab;

    private float         _damage;
    private float         _interval;
    private float         _radius;
    private ActorContext _ctx;

    /*──────────── public API ───────────*/
    public void Init(float dmg, float interval, float radius, float life, ActorContext ctx)
    {
        _damage    = dmg;
        _interval  = interval;
        _radius    = radius;
        _ctx       = ctx;

        if (_idleVfx) _idleVfx.Play();
        StartCoroutine(WorkRoutine());
        Destroy(gameObject, life);
    }

    /*──────────── main loop ───────────*/
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

    /*──────────── helpers ───────────*/
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
        var proj = Instantiate(_projectilePrefab,
                               transform.position + dir * 0.6f,
                               Quaternion.LookRotation(dir));

        // инициализируем без поиска (прямолинейный полёт)
        proj.SetHoming(false);
        proj.EnableSmallExplosion(false);

        float dmg = _damage;
        SkillDamageType type = SkillDamageType.Basic;
        _ctx.ApplyDamageModifiers(ref dmg, ref type);

        proj.Init(dmg, 2f, type, _ctx);        // lifetime 2 с

        // моментальный хит-скан
        target.ReceiveDamage(dmg, type);
        _ctx.FireOnDamageDealt(target, dmg, type);
    }
}