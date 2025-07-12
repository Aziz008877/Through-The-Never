using UnityEngine;
public sealed class DragonsFavorPassive : PassiveSkillBehaviour, IDamageModifier
{
    [Header("Shield formula")]
    [SerializeField] private float _shieldPerSecond = 8f;
    [SerializeField] private ParticleSystem _shieldVfx;
    private float _shieldHp;
    private ActiveSkillBehaviour _special;
    public override void EnablePassive()
    {
        AttachTo(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Special));
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        PlayerContext.RegisterModifier(this);
    }

    public override void DisablePassive()
    {
        Detach();
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        PlayerContext.UnregisterModifier(this);
        StopVfx();
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Special) AttachTo(beh);
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
    
    private void CreateShield(float cdSeconds)
    {
        _shieldHp = cdSeconds * _shieldPerSecond;

        if (_shieldVfx != null)
        {
            _shieldVfx.transform.SetParent(PlayerContext.PlayerPosition);
            _shieldVfx.transform.localPosition = Vector3.zero;
            _shieldVfx.Play(true);
        }
    }

    private void StopVfx()
    {
        if (_shieldVfx != null) _shieldVfx.Stop(true,
            ParticleSystemStopBehavior.StopEmitting);
    }
    
    public void Apply(ref float dmg, ref SkillDamageType type)
    {
        if (_shieldHp <= 0f) return;

        float absorbed = Mathf.Min(dmg, _shieldHp);
        _shieldHp -= absorbed;
        dmg -= absorbed;

        if (_shieldHp <= 0f) StopVfx();
    }
}
