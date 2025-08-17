using UnityEngine;

public class IceBasicAttackprojectile : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] protected ParticleSystem _castVfx;
    [SerializeField] protected ParticleSystem _flightVfx;
    [SerializeField] protected ParticleSystem _hitVfx;

    [Header("Movement")]
    [SerializeField] protected float _speed = 70f;
    
    protected float _instantDamage;
    protected SkillDamageType _damageType;
    protected ActorContext _context;
    protected Vector3 _dir;
    protected bool _canDamage = true;
    
    public void Init(float damage, float lifeTime, SkillDamageType type, ActorContext context)
    {
        _instantDamage = damage;
        _damageType = type;
        _context = context;

        _dir = transform.forward;
        if (_castVfx) _castVfx.Play();
        Invoke(nameof(DestroySelf), lifeTime);
    }
    
    private void DestroySelf()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }
}
