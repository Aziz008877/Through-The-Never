using UnityEngine;
public sealed class DragonsFavorPassive : PassiveSkillBehaviour
{
    [Header("Shield formula")]
    [SerializeField] private float _shieldPerSecond = 8f;
    [SerializeField] private ParticleSystem _shieldVfx;
    private ActiveSkillBehaviour _special;
    private float _shieldHp;
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        AttachTo(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Special));
        PlayerContext.PlayerHp.OnIncomingDamage += AbsorbDamage;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        PlayerContext.PlayerHp.OnIncomingDamage -= AbsorbDamage;

        Detach();
        StopVfx();
        _shieldHp = 0f;
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Special)
            AttachTo(beh);
    }

    private void AttachTo(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh == null) return;

        _special = beh;
        _special.OnCooldownStarted += CreateShield;
    }

    private void Detach()
    {
        if (_special != null)
        {
            _special.OnCooldownStarted -= CreateShield;
            _special = null;
        }
    }

    private void CreateShield(float cooldownSeconds)
    {
        if (_shieldHp > 0f) StopVfx();

        _shieldHp = cooldownSeconds * _shieldPerSecond;
        
        if (_shieldVfx != null)
        {
            _shieldVfx.transform.SetParent(PlayerContext.PlayerPosition, false);
            _shieldVfx.transform.localPosition = Vector3.zero;
            _shieldVfx.Play(true);
        }
    }

    private void StopVfx()
    {
        if (_shieldVfx != null && _shieldVfx.isPlaying)
            _shieldVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void AbsorbDamage(ref float dmg, IDamageable source)
    {
        if (_shieldHp <= 0f) return;

        float absorbed = Mathf.Min(dmg, _shieldHp);
        _shieldHp -= absorbed;
        dmg -= absorbed;

        if (_shieldHp <= 0f)
        {
            StopVfx();
        }
    }
}
