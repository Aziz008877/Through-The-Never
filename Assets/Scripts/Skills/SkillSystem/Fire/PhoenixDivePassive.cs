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
    [SerializeField] private float _distanceMultiplier = 1.4f;
    [SerializeField] private float _cooldownMultiplier = 1.3f;
    private PlayerSkillManager _playerSkillManager;
    private PlayerDashSkill _dash;
    public override void EnablePassive()
    {
        _playerSkillManager = PlayerContext.PlayerSkillManager;
        Attach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Dash));
        _playerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        _playerSkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh != null && beh.TryGetComponent<PlayerDashSkill>(out var dash))
        {
            _dash = dash;
            _dash.OnDashStarted += DiveStart;
            _dash.OnDashEnded += DiveEnd;
        }
    }

    private void Detach()
    {
        if (_dash != null)
        {
            _dash.OnDashStarted -= DiveStart;
            _dash.OnDashEnded -= DiveEnd;
            _dash = null;
        }
    }

    private void DiveStart(Vector3 startPosition)
    {
        if (_diveVfx != null)
        {
            _diveVfx.transform.SetParent(PlayerContext.PlayerPosition);
            _diveVfx.transform.localPosition = Vector3.zero;
            _diveVfx.Play(true);
        }
    }

    private void DiveEnd(Vector3 endPosition)
    {
        if (_diveVfx != null) _diveVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (_impactVfx != null)
        {
            _impactVfx.transform.position = PlayerContext.PlayerPosition.position;
            _impactVfx.Play(true);
        }

        Collider[] hits = Physics.OverlapSphere(PlayerContext.PlayerPosition.position, _impactRadius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable target)) continue;

            float dmg  = _impactDamage;
            SkillDamageType type = SkillDamageType.Basic;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
            PlayerContext.FireOnDamageDealt(target, dmg, type);
        }
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Cooldown)
            return currentValue * _cooldownMultiplier;
        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Range)
            return currentValue * _distanceMultiplier;
        return currentValue;
    }
}
