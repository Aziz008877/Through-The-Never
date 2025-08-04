using UnityEngine;

public class EmberGuardSkill : ActiveSkillBehaviour, ISkillModifier, IDefenceDurationSkill
{
    [Header("Ember Guard Stats")]
    [SerializeField] private float _damageBonus = 0.35f;
    [SerializeField] private float _damageReduction = 0.35f;
    [SerializeField] private float _reflectPercent = 0.75f;
    [SerializeField] private float _selfSlow = 0.5f;
    [SerializeField] private ParticleSystem _guardVfx;
    [SerializeField] private float _toggleCooldown = 2f;
    private bool _active;
    public event System.Action OnDefenceStarted;
    public event System.Action OnDefenceFinished;
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
        Context.SkillModifierHub.Register(this);
        Context.Hp.OnIncomingDamage += OnIncomingDamage;

        if (_guardVfx) _guardVfx.Play();
        Context.Move.SetSpeedMultiplier(_selfSlow);

        Debug.Log("<color=orange>[Ember Guard]</color> ACTIVATED");
        OnDefenceStarted?.Invoke();
    }

    private void Deactivate()
    {
        _active = false;
        Context.SkillModifierHub.Unregister(this);
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;

        if (_guardVfx) _guardVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Context.Move.SetSpeedMultiplier(1f);

        Debug.Log("<color=orange>[Ember Guard]</color> deactivated");
        OnDefenceFinished?.Invoke();
    }

    private void OnDisable()
    {
        if (_active) Deactivate();
    }
    
    private void OnIncomingDamage(ref float dmg, IDamageable source)
    {
        if (!_active || dmg <= 0f) return;

        float original = dmg;
        
        float reflected = dmg * _reflectPercent;
        SkillDamageType reflectType = SkillDamageType.Basic;

        Context.ApplyDamageModifiers(ref reflected, ref reflectType);
        dmg *= 1f - _damageReduction;

        float radius = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), 5f);

        Collider[] hits = Physics.OverlapSphere(Context.transform.position, radius);
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable enemy)) continue;

            enemy.ReceiveDamage(reflected, reflectType);
            Context.FireOnDamageDealt(enemy, reflected, reflectType);
        }

        Debug.Log(
            $"<color=orange>[Ember Guard]</color> incoming {original:F0} → " +
            $"reduced {dmg:F0} (-{_damageReduction:P0}), " +
            $"reflected {reflected:F0}");
    }
    
    public float Evaluate(SkillKey key, float value)
    {
        if (_active && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
            return value * (1f + _damageBonus);

        return value;
    }
}
