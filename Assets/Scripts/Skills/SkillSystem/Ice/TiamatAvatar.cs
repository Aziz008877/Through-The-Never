using System.Collections;
using UnityEngine;

public class TiamatAvatar : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float _attackInterval = 1f;
    [SerializeField] private TiamatProjectile _projectilePrefab;
    private ActorContext _ownerContext;
    private float _attackDamage;
    private float _attackRadius;
    private float _lifetime;
    private Coroutine _attackRoutine;

    public void Initialize(ActorContext ownerContext, float damage, float radius, float lifetimeSeconds)
    {
        _ownerContext = ownerContext;
        _attackDamage = damage;
        _attackRadius = radius;
        _lifetime = lifetimeSeconds;

        if (_attackRoutine != null) StopCoroutine(_attackRoutine);
        _attackRoutine = StartCoroutine(AttackLoop());

        Destroy(gameObject, _lifetime);
    }

    private IEnumerator AttackLoop()
    {
        var wait = new WaitForSeconds(_attackInterval);

        while (true)
        {
            AttackAllEnemiesInRadius();
            yield return wait;
        }
    }
    
    private void AttackAllEnemiesInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRadius);

        foreach (var col in colliders)
        {
            if (!col.TryGetComponent<IDamageable>(out var damageable) || !damageable.CanBeDamaged)
                continue;

            ShootProjectile(damageable);
        }
    }

    private void ShootProjectile(IDamageable target)
    {
        var projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(_ownerContext, target, _attackDamage);
    }
}
