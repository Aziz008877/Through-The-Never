using System.Collections;
using UnityEngine;
public class FireTrailPuddle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _vfx;
    private float _damage, _tickRate, _radius;
    private PlayerContext _playerContext;

    public void Init(float dmg, float tickRate, float radius, float lifeTime, PlayerContext ctx)
    {
        _damage = dmg;
        _tickRate = tickRate;
        _radius = radius;
        _playerContext = ctx;
        
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = radius;

        if (_vfx) _vfx.Play();
        StartCoroutine(TickRoutine());
        Destroy(gameObject, lifeTime);
    }
    
    private IEnumerator TickRoutine()
    {
        var wait = new WaitForSeconds(_tickRate);
        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            foreach (var h in hits)
            {
                if (!h.TryGetComponent(out IDamageable d)) continue;

                float dmg   = _damage;
                SkillDamageType type = SkillDamageType.Basic;
                _playerContext.ApplyDamageModifiers(ref dmg, ref type);
                d.ReceiveDamage(dmg, type);
                _playerContext.FireOnDamageDealt(d, dmg, type);
            }
            yield return wait;
        }
    }
}
