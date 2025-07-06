using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    /* ───────── VFX ───────── */
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castVfx;
    [SerializeField] private ParticleSystem _flightVfx;
    [SerializeField] private ParticleSystem _hitVfx;

    /* ───────── Movement ───── */
    [Header("Movement")]
    [SerializeField] private float _speed = 25f;

    /* ───────── DOT ────────── */
    [Header("DOT")]
    [SerializeField] private float _dotPerSecond = 2f;
    [SerializeField] private float _dotDuration  = 3f;

    /* ───────── Small explosion ─ */
    [Header("Small explosion (Fireblast)")]
    [SerializeField] private float _smallRadius    = 2f;
    [SerializeField] private float _smallDamageMul = 0.5f;
    private bool _smallExplosionEnabled;

    /* ───────── Seeking ─────── */
    [Header("Seeking")]
    [SerializeField] private float _turnSpeedDeg = 240f;
    [SerializeField] private float _seekRadius   = 12f;
    [SerializeField] private float _hitDistance  = 0.3f;
    private bool _homingEnabled;

    /* ───────── Runtime data ── */
    protected float         _instantDamage;
    private   SkillDamageType _damageType;
    protected PlayerContext  _context;
    private   bool           _canDamage = true;
    private   IDamageable    _currentTarget;

    /* ───────── API ─────────── */
    public void EnableSmallExplosion(bool state) => _smallExplosionEnabled = state;
    public void SetHoming(bool state)            => _homingEnabled         = state;

    public void Init(float damage, float lifeTime, SkillDamageType type, PlayerContext context)
    {
        _instantDamage = damage;
        _damageType    = type;
        _context       = context;

        _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);

        if (_homingEnabled) _currentTarget = FindClosestEnemy();
    }

    /* ───────── Update ──────── */
    private void Update()
    {
        if (_homingEnabled)
        {
            // цель ещё не выбрана – пробуем найти самую близкую в радиусе
            if (_currentTarget == null)
                _currentTarget = FindClosestEnemy();

            if (_currentTarget != null)
                SteerTowardsTarget();
        }

        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    /* ───────── Homing ─────── */
    private void AdjustDirection()
    {
        // цель потерялась или вышла из радиуса – ищем новую
        if (_currentTarget == null ||
            ((MonoBehaviour)_currentTarget).gameObject == null ||
            (transform.position - ((MonoBehaviour)_currentTarget).transform.position).sqrMagnitude >
            _seekRadius * _seekRadius)
        {
            _currentTarget = FindClosestEnemy();
            if (_currentTarget == null) return;
        }

        Vector3 toTarget = ((MonoBehaviour)_currentTarget).transform.position - transform.position;

        // совсем близко – считаем попадание
        if (toTarget.sqrMagnitude < _hitDistance * _hitDistance)
        {
            OnTriggerEnter(((MonoBehaviour)_currentTarget).GetComponent<Collider>());
            return;
        }

        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, desired, _turnSpeedDeg * Time.deltaTime);
    }
    
    void SteerTowardsTarget()
    {
        Vector3 toTarget = ((MonoBehaviour)_currentTarget).transform.position - transform.position;

        // если уже «дотронулись» – вручную вызываем попадание
        if (toTarget.sqrMagnitude < _hitDistance * _hitDistance)
        {
            OnTriggerEnter(((MonoBehaviour)_currentTarget).GetComponent<Collider>());
            return;
        }

        Quaternion want = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, want, _turnSpeedDeg * Time.deltaTime);
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

    /* ───────── Collision ───── */
    private void OnTriggerEnter(Collider other)
    {
        if (!_canDamage || !other.TryGetComponent(out IDamageable tgt)) return;

        float dmg  = _instantDamage;
        SkillDamageType type = _damageType;
        _context.ApplyDamageModifiers(ref dmg, ref type);
        tgt.ReceiveDamage(dmg, type);

        if (other.TryGetComponent(out IDotReceivable dot))
            dot.ApplyDot(_dotPerSecond, _dotDuration);

        HitAndStop();
    }

    /* ───────── Impact ──────── */
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

    /* ───────── Destroy ─────── */
    private void DestroySelf()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}
