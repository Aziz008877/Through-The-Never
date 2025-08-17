using System.Collections.Generic;
using UnityEngine;
public sealed class WinterBlastPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _freezeDuration = 2f;
    [SerializeField] private ParticleSystem _blastVfx;
    private ActiveSkillBehaviour _defenceSkill;
    private IDefenceDurationSkill _defenceIface;

    public override void EnablePassive()
    {
        Attach(Context.SkillManager.GetActive(SkillSlot.Defense));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (!beh) return;
        _defenceSkill = beh;

        if (_defenceSkill.TryGetComponent(out _defenceIface) && _defenceIface != null)
        {
            _defenceIface.OnDefenceStarted += TriggerBlast;
        }
        else
        {
            _defenceSkill.OnCooldownStarted += OnDefenceActivatedFallback;
        }
    }

    private void Detach()
    {
        if (_defenceIface != null)
        {
            _defenceIface.OnDefenceStarted -= TriggerBlast;
            _defenceIface = null;
        }

        if (_defenceSkill != null)
        {
            _defenceSkill.OnCooldownStarted -= OnDefenceActivatedFallback;
            _defenceSkill = null;
        }
    }

    private void OnDefenceActivatedFallback(float _)
    {
        TriggerBlast();
    }

    private readonly HashSet<IDamageable> _seen = new();

    private void TriggerBlast()
    {
        if (_blastVfx)
        {
            _blastVfx.transform.position = Context.transform.position;
            _blastVfx.Play();
        }
        
        var hits = Physics.OverlapSphere(Context.transform.position, Definition.Raduis);

        _seen.Clear();
        foreach (var col in hits)
        {
            var dmg = col.GetComponentInParent<IDamageable>();
            if (dmg == null) continue;
            if (!_seen.Add(dmg)) continue;

            if (col.GetComponentInParent<StunDebuff>() is { } stun)
                stun.ApplyStun(_freezeDuration);
        }
    }
}
