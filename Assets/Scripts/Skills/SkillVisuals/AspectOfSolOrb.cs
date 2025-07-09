using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AspectOfSolOrb : MonoBehaviour
{
    [SerializeField] private ParticleSystem _orbVfx;
    [SerializeField] private Fireball _projectilePrefab;
    private float _damage, _projSpeed, _fireRate, _radius;
    private PlayerContext _ctx;

    public void Init(float dmg, float speed, float rate, float radius, float life, PlayerContext ctx)
    {
        _damage = dmg;
        _projSpeed = speed;
        _fireRate = rate;
        _radius = radius;
        _ctx = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = _radius;

        if (_orbVfx) _orbVfx.Play();

        StartCoroutine(FireLoop());
        Destroy(gameObject, life);
    }

    private IEnumerator FireLoop()
    {
        var wait = new WaitForSeconds(_fireRate);

        while (true)
        {
            IDamageable target = FindClosestEnemy();
            if (target != null)
            {
                Vector3 dir = ((MonoBehaviour)target).transform.position - transform.position;
                dir = dir.normalized;

                var proj = Instantiate(_projectilePrefab, transform.position + dir * 1.2f, Quaternion.LookRotation(dir));
                proj.Init(_damage, 3f, SkillDamageType.Basic, _ctx);
                proj.GetComponent<Rigidbody>()?.AddForce(dir * _projSpeed, ForceMode.VelocityChange);
            }
            yield return wait;
        }
    }

    private IDamageable FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
        IDamageable best = null; float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable d)) continue;
            float sqr = (h.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr) { best = d; bestSqr = sqr; }
        }
        return best;
    }
}