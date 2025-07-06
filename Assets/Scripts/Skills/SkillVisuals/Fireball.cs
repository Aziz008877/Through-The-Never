using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castParticles;
    [SerializeField] private ParticleSystem _fireballParticles;
    [SerializeField] private ParticleSystem _fireballHit;

    [Header("Movement")]
    [SerializeField] private float _speed = 25f;

    [Header("DOT data")]
    [SerializeField] protected float _dotPerSecond = 2f;
    [SerializeField] protected float _dotDuration = 3f;
    
    [Header("Small explosion (Fireblast)")]
    [SerializeField] private float _smallExplosionRadius = 2f;
    [SerializeField] private float _smallExplosionMul   = 0.5f;
    private bool _enableSmallExplosion;
    
    protected float _instantDamage;
    private SkillDamageType _skillDamageType;
    protected PlayerContext _context;
    private bool _canDamage = true;

    public void EnableSmallExplosion(bool state) => _enableSmallExplosion = state;
    
    public void Init(float damage,float lifeTime,SkillDamageType type,PlayerContext ctx)
    {
        _instantDamage = damage;
        _skillDamageType = type;
        _context = ctx;
        _castParticles.Play();
        Invoke(nameof(DestroyFireball), lifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canDamage) return;
        if (!other.TryGetComponent(out IDamageable target)) return;

        float dmg = _instantDamage;
        SkillDamageType type = _skillDamageType;
        _context.ApplyDamageModifiers(ref dmg, ref type);
        target.ReceiveDamage(dmg, type);

        if (other.TryGetComponent(out IDotReceivable dotTarget))
            dotTarget.ApplyDot(_dotPerSecond, _dotDuration);

        HitAndStop();
    }

    protected virtual void HitAndStop()
    {
        if (_enableSmallExplosion) SmallExplode();
        _canDamage = false;
        _fireballParticles.gameObject.SetActive(false);
        _fireballHit.Play();
        _speed = 0f;
        Invoke(nameof(DestroyFireball), _dotDuration + 0.5f);
    }

    private void SmallExplode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _smallExplosionRadius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable damageable)) continue;

            float damage = _instantDamage * _smallExplosionMul;
            Debug.Log("Explosion Damage");
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref damage, ref type);
            damageable.ReceiveDamage(damage, type);
        }
    }
    
    protected virtual void DestroyFireball()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}