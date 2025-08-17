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