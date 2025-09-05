using System.Collections;
using UnityEngine;

public class IceSpireTotem : MonoBehaviour, IDamageContextModifier, IOnDamageDealtContextModifier
{
    private ActorContext _owner;
    private IceSpireSkill _skill;
    private float _radius;
    private float _tick;
    private float _push;
    private LayerMask _enemyMask;
    private LayerMask _projectileMask;
    private float _mult;
    private float _kb;
    private ParticleSystem _loopVfx;

    private Coroutine _routine;

    public void Init(ActorContext owner, IceSpireSkill skill, float duration, float radius, float tick, float pushForce,
                     LayerMask enemyMask, LayerMask projectileMask, float dmgMult, float onHitKb, ParticleSystem loopVfx)
    {
        _owner = owner;
        _skill = skill;
        _radius = radius;
        _tick = Mathf.Max(0.05f, tick);
        _push = pushForce;
        _enemyMask = enemyMask;
        _projectileMask = projectileMask;
        _mult = Mathf.Max(1f, dmgMult);
        _kb = Mathf.Max(0f, onHitKb);
        _loopVfx = loopVfx;

        if (_loopVfx)
        {
            _loopVfx.transform.position = transform.position;
            _loopVfx.Play();
        }

        _owner.RegisterContextModifier(this);
        _owner.RegisterOnDamageDealtContextModifier(this);

        _routine = StartCoroutine(LifeRoutine(duration));
    }

    private IEnumerator LifeRoutine(float dur)
    {
        var wait = new WaitForSeconds(_tick);
        float t = dur;
        while (t > 0f)
        {
            AuraTick();
            t -= _tick;
            yield return wait;
        }
        Cleanup();
    }

    private void AuraTick()
    {
        Vector3 o = transform.position;

        var enemies = Physics.OverlapSphere(o, _radius, _enemyMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < enemies.Length; i++)
        {
            var col = enemies[i];
            if (!col.transform || col.transform == _owner.transform) continue;

            Vector3 dir = (col.transform.position - o);
            dir.y = 0f;
            float len = dir.magnitude;
            if (len < 0.001f) dir = (col.transform.position - _owner.transform.position).normalized;
            else dir /= len;

            if (col.attachedRigidbody)
                col.attachedRigidbody.AddForce(dir * _push, ForceMode.Acceleration);
            else if (col.TryGetComponent(out IKnockbackable kb))
                kb.ApplyKnockback(dir, _push);
        }

        if (_projectileMask.value != 0)
        {
            var projs = Physics.OverlapSphere(o, _radius, _projectileMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < projs.Length; i++)
            {
                var c = projs[i];
                if (c.TryGetComponent(out IProjectile p))
                    p.DestroyProjectile();
                else
                    Destroy(c.attachedRigidbody ? c.attachedRigidbody.gameObject : c.gameObject);
            }
        }
    }

    public void Apply(ref DamageContext ctx)
    {
        if (ctx.Attacker != _owner) return;
        if (!IsOwnerInside()) return;
        ctx.Damage *= _mult;
    }

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Attacker != _owner) return;
        if (!IsOwnerInside()) return;
        if (ctx.Target is Component comp)
        {
            Vector3 dir = (comp.transform.position - _owner.transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = (comp.transform.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) dir = _owner.transform.forward;
            dir.Normalize();

            if (comp.TryGetComponent<Rigidbody>(out var rb))
                rb.AddForce(dir * _kb, ForceMode.VelocityChange);
            else if (comp.TryGetComponent<IKnockbackable>(out var kb))
                kb.ApplyKnockback(dir, _kb);
        }
    }

    private bool IsOwnerInside()
    {
        if (!_owner) return false;
        Vector3 a = _owner.transform.position; a.y = 0f;
        Vector3 b = transform.position;        b.y = 0f;
        return (a - b).sqrMagnitude <= _radius * _radius;
    }

    private void Cleanup()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (_loopVfx) _loopVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (_owner != null)
        {
            _owner.UnregisterContextModifier(this);
            _owner.UnregisterOnDamageDealtContextModifier(this);
        }

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (_owner != null)
        {
            _owner.UnregisterContextModifier(this);
            _owner.UnregisterOnDamageDealtContextModifier(this);
        }
    }
}

public interface IKnockbackable
{
    void ApplyKnockback(Vector3 dir, float force);
}

public interface IProjectile
{
    void DestroyProjectile();
}
