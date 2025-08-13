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

        Vector3 targetPos = ((MonoBehaviour) _target).transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= _hitRadius)
        {
            Impact();
        }
    }

    private void Impact()
    {
        SkillDamageType type = SkillDamageType.Basic;
        float finalDamage = _damage;

        _ownerContext.ApplyDamageModifiers(ref finalDamage, ref type);
        _target.ReceiveDamage(finalDamage, type);
        _ownerContext.FireOnDamageDealt(_target, finalDamage, type);

        if (_target.CurrentHP <= _target.MinHP)
            _ownerContext.Hp.ReceiveHP(finalDamage * 0.5f);

        if (_impactVfx) Instantiate(_impactVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}