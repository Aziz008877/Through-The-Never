using UnityEngine;

public class TiamatProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private GameObject _impactVfx;
    [SerializeField] private float _hitRadius = 0.3f;

    private ActorContext _ownerContext;
    private IDamageable _target;
    private float _damage;

    public void Launch(ActorContext ownerContext, IDamageable target, float damage)
    {
        _ownerContext = ownerContext;
        _target = target;
        _damage = damage;
    }

    private void Update()
    {
        if (_target == null || !_target.CanBeDamaged)
        {
            Destroy(gameObject);
            return;
        }
        
        var targetComp = _target as Component;
        if (targetComp == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = targetComp.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= _hitRadius)
        {
            Impact(targetPos);
        }
    }

    private void Impact(Vector3 hitPoint)
    {
        var ctx = new DamageContext
        {
            Attacker = _ownerContext,
            Target = _target,
            SkillBehaviour = null,
            SkillDef = null,
            Slot = SkillSlot.Special,
            Type = SkillDamageType.Basic,
            Damage = _damage,
            IsCrit = false,
            CritMultiplier = 1f,
            HitPoint = hitPoint,
            HitNormal = Vector3.up,
            SourceGO = gameObject
        };
        
        _ownerContext.ApplyDamageContextModifiers(ref ctx);
        
        _target.ReceiveDamage(ctx);
        
        if (_target.CurrentHP <= _target.MinHP)
            _ownerContext.Hp.ReceiveHP(ctx.Damage * 0.5f);

        if (_impactVfx) Instantiate(_impactVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
