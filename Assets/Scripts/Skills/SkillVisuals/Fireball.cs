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
    [SerializeField] private float _dotPerSecond = 2f;
    [SerializeField] private float _dotDuration = 3f;

    private float _instantDamage;
    private SkillDamageType _skillDamageType;
    private PlayerContext _context;
    private bool _canDamage = true;

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

    private void HitAndStop()
    {
        _canDamage = false;
        _fireballParticles.gameObject.SetActive(false);
        _fireballHit.Play();
        _speed = 0f;
        Invoke(nameof(DestroyFireball), _dotDuration + 0.5f);
    }

    private void DestroyFireball()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}