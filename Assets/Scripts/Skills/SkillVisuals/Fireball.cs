using UnityEngine;

public class Fireball : MonoBehaviour, IProjectileBoostable
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castVfx;
    [SerializeField] private ParticleSystem _flightVfx;
    [SerializeField] private ParticleSystem _hitVfx;

    [Header("Movement")]
    [SerializeField] private float _speed = 25f;

    [Header("DOT")]
    [SerializeField] private float _dotPerSecond = 2f;
    [SerializeField] private float _dotDuration = 3f;
    [SerializeField] private float _damageMul = 1f;

    [Header("Small explosion (Fireblast)")]
    [SerializeField] private float _smallRadius = 2f;
    [SerializeField] private float _smallDamageMul = 0.5f;
    private bool _smallExplosionEnabled;

    [Header("Seeking")]
    [SerializeField] private float _turnSpeedDeg = 240f;
    [SerializeField] private float _seekRadius = 12f;
    [SerializeField] private float _hitDistance = 0.3f;
    private bool _homingEnabled;

    protected float _instantDamage;
    private SkillDamageType _damageType;
    protected PlayerContext _context;
    private bool _canDamage = true;
    private IDamageable _target;
    private Vector3 _moveDirection;
    private float _startDamage;

    public void EnableSmallExplosion(bool state) => _smallExplosionEnabled = state;
    public void SetHoming(bool state) => _homingEnabled = state;

    public void Init(float damage, float lifeTime, SkillDamageType type, PlayerContext context)
    {
        _startDamage = damage;
        _instantDamage = _startDamage * _damageMul;
        _damageType = type;
        _context = context;

        _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);

        if (_homingEnabled)
        {
            _target = FindClosestEnemy();
            if (_target != null)
            {
                Vector3 toTarget = ((MonoBehaviour)_target).transform.position - transform.position;
                _moveDirection = toTarget.normalized;
                transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            }
            else
            {
                _moveDirection = transform.forward;
            }
        }
        else
        {
            _moveDirection = transform.forward;
        }
    }

    private void Update()
    {
        if (_homingEnabled && _target != null)
        {
            var tgtMb = (MonoBehaviour)_target;
            
            if (tgtMb == null || tgtMb.gameObject == null)
            {
                _target = null;
            }
            else
            {
                Vector3 toTarget = tgtMb.transform.position - transform.position;

                if (toTarget.sqrMagnitude < _hitDistance * _hitDistance)
                {
                    OnTriggerEnter(tgtMb.GetComponent<Collider>());
                    return;
                }
                
                Quaternion current = transform.rotation;
                Quaternion want = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    current, want, _turnSpeedDeg * Time.deltaTime);
                
                _moveDirection = transform.forward;
            }
        }

        transform.position += _moveDirection * _speed * Time.deltaTime;
    }

    private IDamageable FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _seekRadius);
        IDamageable best = null; float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable dmg)) continue;
            float sqr = (h.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr) { best = dmg; bestSqr = sqr; }
        }
        return best;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canDamage || !other.TryGetComponent(out IDamageable tgt)) return;

        float dmg = _instantDamage;
        SkillDamageType type = _damageType;
        _context.ApplyDamageModifiers(ref dmg, ref type);
        tgt.ReceiveDamage(dmg, type);

        _context.FireOnDamageDealt(tgt, dmg, type);

        HitAndStop();
    }

    protected virtual void HitAndStop()
    {
        if (_smallExplosionEnabled) SmallExplode();

        _canDamage = false;
        _flightVfx.gameObject.SetActive(false);
        _hitVfx.Play();
        _speed = 0f;

        Invoke(nameof(DestroySelf), _dotDuration + 0.5f);
    }

    private void SmallExplode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _smallRadius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable tgt)) continue;

            float dmg = _instantDamage * _smallDamageMul;
            SkillDamageType type = SkillDamageType.Basic;
            _context.ApplyDamageModifiers(ref dmg, ref type);
            tgt.ReceiveDamage(dmg, type);
        }
    }

    private void DestroySelf()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }

    public void BoostDamage(float mul)
    {
        _damageMul *= Mathf.Max(1f, mul);
        _instantDamage = _startDamage * _damageMul;
    }
}
