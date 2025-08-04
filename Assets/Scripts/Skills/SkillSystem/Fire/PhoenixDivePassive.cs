using System.Collections.Generic;
using UnityEngine;

public sealed class PhoenixDivePassive : PassiveSkillBehaviour, ISkillModifier
{
    [Header("Visuals")]
    [SerializeField] private ParticleSystem _diveVfx;
    [SerializeField] private ParticleSystem _impactVfx;

    [Header("Explosion")]
    [SerializeField] private float _impactRadius  = 3f;
    [SerializeField] private float _impactDamage  = 20f;

    [Header("Dash tuning")]
    [SerializeField] private float _distanceMultiplier  = 1.4f;
    [SerializeField] private float _cooldownMultiplier  = 1.3f;
    private ISkillManager _mgr;
    private PlayerDashSkill    _dash;

    /* ───────── Enable / Disable ───────── */
    public override void EnablePassive()
    {
        _mgr = Context.SkillManager;
        Attach(_mgr.GetActive(SkillSlot.Dash));
        _mgr.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        _mgr.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    /* ───────── подписка на Dash ───────── */
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashStarted += DiveStart;
            _dash.OnDashEnded   += DiveEnd;
        }
    }

    private void Detach()
    {
        if (_dash)
        {
            _dash.OnDashStarted -= DiveStart;
            _dash.OnDashEnded   -= DiveEnd;
            _dash = null;
        }
    }

    /* ───────── начало / конец рывка ───────── */
    private void DiveStart(Vector3 pos)
    {
        if (_diveVfx)
        {
            _diveVfx.transform.SetParent(Context.ActorPosition, false);
            _diveVfx.transform.localPosition = Vector3.zero;
            _diveVfx.Play(true);
        }

        Context.FireballModel.SetActive(true);
        foreach (var r in Context.PlayerMeshes) r.enabled = false;
    }

    private void DiveEnd(Vector3 pos)
    {
        if (_diveVfx)
            _diveVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // ◄ вернуться к обычной модели
        Context.FireballModel.SetActive(false);
        foreach (var r in Context.PlayerMeshes) r.enabled = true;

        if (_impactVfx)
        {
            _impactVfx.transform.position = Context.ActorPosition.position;
            _impactVfx.Play(true);
        }

        float radius = Context.SkillModifierHub
                       .Apply(new SkillKey(Definition.Slot, SkillStat.Radius), _impactRadius);

        int hitCount = 0;
        Collider[] hits = Physics.OverlapSphere(Context.ActorPosition.position, radius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable target)) continue;

            float dmg  = _impactDamage;
            SkillDamageType type = SkillDamageType.Basic;          // 🔥

            Context.ApplyDamageModifiers(ref dmg, ref type);
            target.ReceiveDamage(dmg, type);                      // ⚠️  НАНОСИМ УРОН
            Context.FireOnDamageDealt(target, dmg, type);

            hitCount++;
        }

        Debug.Log($"<color=orange>[Phoenix Dive]</color> dealt {_impactDamage} to {hitCount} target(s)");
    }

    /* ───────── ISkillModifier ───────── */
    public float Evaluate(SkillKey key, float value)
    {
        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Cooldown)
            return value * _cooldownMultiplier;

        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Range)
            return value * _distanceMultiplier;

        return value;
    }
}
