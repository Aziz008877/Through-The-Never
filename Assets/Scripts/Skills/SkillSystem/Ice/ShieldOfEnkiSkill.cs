using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldOfEnkiSkill : ActiveSkillBehaviour, ISkillModifier, IDefenceDurationSkill
{
    [Header("Toggle / Cooldown")]
    [SerializeField] private float _toggleCooldown = 2f;

    [Header("Front Block")]
    [SerializeField] private float _blockAngleDeg = 110f;
    [SerializeField] private bool  _blockWhenNoSource = true;

    [Header("Cone Damage")]
    [SerializeField] private float _coneRange   = 6f;
    [SerializeField] private float _coneAngleDeg = 80f;
    [SerializeField] private float _tickDamage   = 22f;
    [SerializeField] private float _tickInterval = 0.35f;
    [SerializeField] private LayerMask _damageMask = ~0;

    [Header("Movement While Active")]
    [SerializeField] private float _selfSlow = 0.7f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _shieldLoopVfx;
    [SerializeField] private ParticleSystem _coneHitVfx;

    private bool _active;
    private Quaternion _lockedRotation;
    private Coroutine _tickRoutine;

    public event Action OnDefenceStarted;
    public event Action OnDefenceFinished;

    public override void TryCast()
    {
        if (!IsReady) return;

        if (_active) Deactivate();
        else Activate();

        _cooldownTimer = _toggleCooldown;
        OnCooldownStarted?.Invoke(_toggleCooldown);
    }

    private void Activate()
    {
        _active = true;

        _lockedRotation = Context.transform.rotation;
        Context.SkillModifierHub.Register(this);
        Context.Hp.OnIncomingDamage += OnIncomingDamageFrontBlock;
        Context.Move.SetSpeedMultiplier(_selfSlow);

        if (_shieldLoopVfx) _shieldLoopVfx.Play();

        OnDefenceStarted?.Invoke();

        if (_tickRoutine != null) StopCoroutine(_tickRoutine);
        _tickRoutine = StartCoroutine(ConeTickRoutine());
    }

    private void Deactivate()
    {
        _active = false;

        Context.SkillModifierHub.Unregister(this);
        Context.Hp.OnIncomingDamage -= OnIncomingDamageFrontBlock;

        Context.Move.SetSpeedMultiplier(1f);

        if (_shieldLoopVfx) _shieldLoopVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (_tickRoutine != null)
        {
            StopCoroutine(_tickRoutine);
            _tickRoutine = null;
        }

        OnDefenceFinished?.Invoke();
    }

    private void OnDisable()
    {
        if (_active) Deactivate();
    }

    private void LateUpdate()
    {
        if (_active)
        {
            Context.transform.rotation = _lockedRotation;
        }
    }
    
    private void OnIncomingDamageFrontBlock(ref float dmg, IDamageable source)
    {
        if (!_active || dmg <= 0f) return;

        Vector3 srcPos;
        bool haveSrc = false;

        if (source is Component c)
        {
            srcPos  = c.transform.position;
            haveSrc = true;
        }
        else
        {
            srcPos = Context.transform.position + Context.transform.forward; 
        }

        if (!haveSrc && !_blockWhenNoSource) return;

        Vector3 toSrc = srcPos - Context.transform.position;
        toSrc.y = 0f;
        if (toSrc.sqrMagnitude < 0.0001f)
        {
            dmg = 0f;
            return;
        }

        toSrc.Normalize();
        float dot     = Vector3.Dot(Context.transform.forward, toSrc);
        float cosHalf = Mathf.Cos(0.5f * _blockAngleDeg * Mathf.Deg2Rad);

        if (dot >= cosHalf)
        {
            dmg = 0f;
        }
    }
    private IEnumerator ConeTickRoutine()
    {
        var wait = new WaitForSeconds(_tickInterval > 0.01f ? _tickInterval : 0.1f);

        while (_active)
        {
            DoConeTick();
            yield return wait;
        }
    }

    private void DoConeTick()
    {
        float range = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), _coneRange);

        Vector3 origin = Context.transform.position;
        Vector3 fwd    = Context.transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, range, _damageMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return;

        float cosHalf = Mathf.Cos(0.5f * _coneAngleDeg * Mathf.Deg2Rad);
        List<IDamageable> targets = new List<IDamageable>(8);

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (!col.TryGetComponent(out IDamageable dmg)) continue;

            Vector3 dir = col.transform.position - origin; dir.y = 0f;
            float sqr = dir.sqrMagnitude;
            if (sqr > range * range || sqr < 0.0001f) continue;

            dir.Normalize();
            float dot = Vector3.Dot(fwd, dir);
            if (dot < cosHalf) continue;

            if (!targets.Contains(dmg))
                targets.Add(dmg);
        }

        if (targets.Count == 0) return;

        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];

            var ctx = BuildDamage(
                _tickDamage,
                SkillDamageType.Basic,
                hitPoint: (t is Component comp) ? comp.transform.position : origin + fwd * (range * 0.5f),
                hitNormal: Vector3.up,
                sourceGO: gameObject
            );
            ctx.Target = t;

            Context.ApplyDamageContextModifiers(ref ctx);
            t.ReceiveDamage(ctx);

            if (_coneHitVfx && t is Component comp2)
            {
                _coneHitVfx.transform.position = comp2.transform.position;
                _coneHitVfx.Play();
            }
        }
    }
    
    public float Evaluate(SkillKey key, float value)
    {
        // Щит сам по себе не даёт бонус урона/редакцию глобально (это чистая защита + удар конусом),
        // но если захочешь, можно добавить, например, +Damage к базовым спеллам, пока щит активен:
        // if (_active && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
        //     return value * 1.0f;
        return value;
    }

    public float GetDefenceDuration() => Mathf.Infinity; 
}
