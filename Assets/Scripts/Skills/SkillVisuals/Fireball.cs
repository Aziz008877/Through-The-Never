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
    protected ActorContext _context;
    private bool _canDamage = true;
    private IDamageable _target;
    private Vector3 _moveDirection;
    private float _startDamage;
    private DamageContext _baseCtx;
    private bool _skipContextModsAtHit;
    public void EnableSmallExplosion(bool state) => _smallExplosionEnabled = state;
    public void SetHoming(bool state) => _homingEnabled = state;
    public void Init(in DamageContext ctx, float lifeTime)
    {
        _baseCtx = ctx;
        _context = ctx.Attacker;
        _startDamage = ctx.Damage;
        _damageType = ctx.Type;
        RecomputeInstantDamage();
        _skipContextModsAtHit = false;

        _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);

        SetupInitialDirection();
    }
    
    public void Init(float damage, float lifeTime, SkillDamageType type, ActorContext context)
    {
        _context = context;
        _startDamage = damage;
        _damageType = type;
        
        _baseCtx = new DamageContext
        {
            Attacker       = _context,
            Target         = null,
            SkillBehaviour = null,
            SkillDef       = null,
            Slot           = SkillSlot.Undefined,
            Type           = _damageType,
            Damage         = _startDamage,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = transform.position,
            HitNormal      = Vector3.up,
            SourceGO       = gameObject
        };

        RecomputeInstantDamage();
        _skipContextModsAtHit = true;

        _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);

        SetupInitialDirection();
    }

    private void RecomputeInstantDamage()
    {
        _instantDamage = _startDamage * _damageMul;
    }

    private void SetupInitialDirection()
    {
        if (_homingEnabled)
        {
            _target = FindClosestEnemy();
            if (_target is Component c && c != null)
            {
                Vector3 toTarget = c.transform.position - transform.position;
                _moveDirection = toTarget.sqrMagnitude > 0f ? toTarget.normalized : transform.forward;
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
        if (_homingEnabled && _target is Component t && t != null)
        {
            Vector3 toTarget = t.transform.position - transform.position;

            if (toTarget.sqrMagnitude < _hitDistance * _hitDistance)
            {
                var col = t.GetComponent<Collider>();
                if (col != null) OnTriggerEnter(col);
                return;
            }

            Quaternion current = transform.rotation;
            Quaternion want    = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(current, want, _turnSpeedDeg * Time.deltaTime);

            _moveDirection = transform.forward;
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
        var apply = _baseCtx;
        apply.Target   = tgt;
        apply.HitPoint = other.ClosestPoint(transform.position);
        apply.Type     = _damageType;
        apply.Damage   = _instantDamage;
        
        if (!_skipContextModsAtHit)
            _context.ApplyDamageContextModifiers(ref apply);

        tgt.ReceiveDamage(apply);
        HitAndStop();
    }

    protected virtual void HitAndStop()
    {
        if (_smallExplosionEnabled) SmallExplode();

        _canDamage = false;
        if (_flightVfx) _flightVfx.gameObject.SetActive(false);
        _hitVfx.Play();
        _speed = 0f;
        Invoke(nameof(DestroySelf), _dotDuration + 0.5f);
    }

    private void SmallExplode()
    {
        var hits = Physics.OverlapSphere(transform.position, _smallRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!h.TryGetComponent(out IDamageable tgt)) continue;

            var aoe = _baseCtx;
            aoe.Target   = tgt;
            aoe.HitPoint = h.transform.position;
            aoe.Type     = SkillDamageType.Basic;
            aoe.Damage   = _instantDamage * _smallDamageMul;

            if (!_skipContextModsAtHit)
                _context.ApplyDamageContextModifiers(ref aoe);

            tgt.ReceiveDamage(aoe);
        }
    }

    private void DestroySelf()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }

    public void BoostDamage(float mul)
    {
        _damageMul = Mathf.Max(1f, mul) * _damageMul;
        RecomputeInstantDamage();
    }
}
