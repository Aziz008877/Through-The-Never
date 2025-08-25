using UnityEngine;

public class RejuvenationSkillPassive : PassiveSkillBehaviour, IOnDamageDealtContextModifier
{
    [SerializeField, Range(0f,1f)] private float _lifestealPercent = 0.3f;
    [SerializeField] private float _creditWindow = 0.6f;
    private ActiveSkillBehaviour _special;
    private float _creditUntil;

    public override void EnablePassive()
    {
        Context.RegisterOnDamageDealtContextModifier(this);

        _special = Context.SkillManager.GetActive(SkillSlot.Special);
        if (_special) _special.OnCooldownStarted += OnSpecialCast;

        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Context.UnregisterOnDamageDealtContextModifier(this);

        if (_special) _special.OnCooldownStarted -= OnSpecialCast;
        _special = null;

        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot != SkillSlot.Special) return;
        if (_special) _special.OnCooldownStarted -= OnSpecialCast;
        _special = beh;
        if (_special) _special.OnCooldownStarted += OnSpecialCast;
    }

    private void OnSpecialCast(float _)
    {
        _creditUntil = Time.time + _creditWindow;
    }

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Damage <= 0f) return;
        if (Time.time > _creditUntil) return;

        float heal = ctx.Damage * _lifestealPercent;
        Context.Hp.ReceiveHP(heal);
    }
}
