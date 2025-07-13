using UnityEngine;
public class EmberGuardSkill : ActiveSkillBehaviour, ISkillModifier
{
    [Header("Ember Guard Stats")]
    [SerializeField] private float _damageBonus = 0.35f;
    [SerializeField] private float _damageReduction = 0.35f;
    [SerializeField] private float _reflectPercent = 0.75f;
    [SerializeField] private float _selfSlow = 0.5f;
    [SerializeField] private ParticleSystem _guardVfx;
    [SerializeField] private float _toggleCooldown = 2f;
    private bool _active;
    public override void TryCast()
    {
        if (!IsReady) return;

        if (_active)
            Deactivate();
        else
            Activate();
        
        _cooldownTimer = _toggleCooldown;
        OnCooldownStarted?.Invoke(_toggleCooldown);
    }

    private void Activate()
    {
        _active = true;
        PlayerContext.SkillModifierHub.Register(this);
        PlayerContext.PlayerHp.OnIncomingDamage += OnIncomingDamage;
        _guardVfx.Play();
        PlayerContext.PlayerMove.SetSpeedMultiplier(_selfSlow);
    }

    private void Deactivate()
    {
        _active = false;
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerHp.OnIncomingDamage -= OnIncomingDamage;
        _guardVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        PlayerContext.PlayerMove.SetSpeedMultiplier(1f);
    }

    private void OnIncomingDamage(ref float dmg)
    {
        if (!_active || dmg <= 0f) return;
        
        float reflected = dmg * _reflectPercent;
        SkillDamageType reflectType = SkillDamageType.Basic;
        PlayerContext.ApplyDamageModifiers(ref reflected, ref reflectType);
        
        dmg *= 1f - _damageReduction;

        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), 5f));
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable enemy)) continue;
            enemy.ReceiveDamage(reflected, reflectType);
            PlayerContext.FireOnDamageDealt(enemy, reflected, reflectType);
        }
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_active && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
        {
            return currentValue * (1f + _damageBonus);
        }
        return currentValue;
    }

    private void OnDisable()
    {
        if (_active)
            Deactivate();
    }
}
