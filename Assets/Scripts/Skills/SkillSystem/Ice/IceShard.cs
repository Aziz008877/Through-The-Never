using UnityEngine;

public class IceShard : IceBasicAttackprojectile
{
    private void Update()
    {
        transform.position += _dir * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canDamage) return;
        if (!other.TryGetComponent(out IDamageable tgt)) return;

        var ctx = new DamageContext
        {
            Attacker       = _context,
            Target         = tgt,
            SkillBehaviour = null,
            SkillDef       = null,
            Slot           = SkillSlot.Basic,
            Type           = _damageType,
            Damage         = _instantDamage,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = other.transform.position,
            HitNormal      = Vector3.up,
            SourceGO       = gameObject
        };

        _context.ApplyDamageContextModifiers(ref ctx);
        tgt.ReceiveDamage(ctx);

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