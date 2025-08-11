using UnityEngine;

public class IceShard : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castVfx;
    [SerializeField] private ParticleSystem _flightVfx;
    [SerializeField] private ParticleSystem _hitVfx;

    [Header("Movement")]
    [SerializeField] private float _speed = 70f;

    [Header("Hit")]
    [SerializeField] private float _hitDistance = 0.2f;

    private float _instantDamage;
    private SkillDamageType _damageType;
    private ActorContext _context;
    private Vector3 _dir;
    private bool _canDamage = true;

    public void Init(float damage, float lifeTime, SkillDamageType type, ActorContext context)
    {
        _instantDamage = damage;
        _damageType = type;
        _context = context;

        _dir = transform.forward;
        if (_castVfx) _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void Update()
    {
        transform.position += _dir * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canDamage) return;
        if (!other.TryGetComponent(out IDamageable tgt)) return;

        float dmg = _instantDamage;
        SkillDamageType type = _damageType;
        _context.ApplyDamageModifiers(ref dmg, ref type);
        tgt.ReceiveDamage(dmg, type);
        _context.FireOnDamageDealt(tgt, dmg, type);

        HitAndStop();
    }

    private void HitAndStop()
    {
        _canDamage = false;
        if (_flightVfx) _flightVfx.gameObject.SetActive(false);
        if (_hitVfx) _hitVfx.Play();
        _speed = 0f;
        DestroySelf();
    }

    private void DestroySelf()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}