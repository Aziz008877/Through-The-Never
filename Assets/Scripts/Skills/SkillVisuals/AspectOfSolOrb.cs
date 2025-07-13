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
        Debug.Log(transform.position);
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
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);

            foreach (var h in hits)
            {
                if (!h.TryGetComponent(out IDamageable enemy)) continue;
                Vector3 dir = (h.transform.position - transform.position).normalized;
                Fireball proj = Instantiate(_projectilePrefab, transform.position + dir * 1.2f, Quaternion.LookRotation(dir));
                proj.Init(_damage, 3f, SkillDamageType.Basic, _ctx);
            }

            yield return wait;
        }
    }
}