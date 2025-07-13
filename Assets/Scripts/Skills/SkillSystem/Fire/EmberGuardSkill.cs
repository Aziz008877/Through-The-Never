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
    private Coroutine _routine;

    public override void TryCast()
    {
        if (!IsReady) return;

        if (_active)
            Deactivate();
        else
            Activate();

        _cooldownTimer = _toggleCooldown;
    }

    private void Activate()
    {
        Debug.Log("Active");
        _active = true;
        PlayerContext.SkillModifierHub.Register(this);
        PlayerContext.PlayerHp.OnPlayerReceivedDamage += OnPlayerDamaged;
        if (_guardVfx) _guardVfx.Play();

        PlayerContext.PlayerMove.SetSpeedMultiplier(_selfSlow);
    }

    private void Deactivate()
    {
        Debug.Log("Inactive");
        _active = false;
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerHp.OnPlayerReceivedDamage -= OnPlayerDamaged;
        if (_guardVfx) _guardVfx.Stop();
        PlayerContext.PlayerMove.SetSpeedMultiplier(1f);
    }
    
    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_active && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
            currentValue *= (1f + _damageBonus);

        return currentValue;
    }
    
    private void OnPlayerDamaged(float dmg)
    {
        if (!_active) return;
        float reflected = dmg * _reflectPercent;
        float reduced   = dmg * (1f - _damageReduction);

        SkillDamageType reflectType = SkillDamageType.Basic;
        PlayerContext.ApplyDamageModifiers(ref reflected, ref reflectType);

        dmg = reduced;

        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, 5f);
        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable enemy)) continue;
            enemy.ReceiveDamage(reflected, reflectType);
            
            PlayerContext.FireOnDamageDealt(enemy, reflected, reflectType);
        }
    }

    private void OnDisable()
    {
        if (_active) Deactivate();
    }
}
