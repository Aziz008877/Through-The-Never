using UnityEngine;
public sealed class ScorchPulsePassive : PassiveSkillBehaviour
{
    [Header("Pulse parameters")]
    [SerializeField] private float _radius = 3f;
    [SerializeField] private float _damage = 12f;
    [SerializeField] private ParticleSystem _pulseVfx;
    private PlayerDashSkill _dash;
    public override void EnablePassive()
    {
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(Context.SkillManager.GetActive(SkillSlot.Dash));
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh != null && beh.TryGetComponent(out _dash))
            _dash.OnDashStarted += SpawnPulse;
    }

    private void Detach()
    {
        if (_dash != null)
        {
            _dash.OnDashStarted -= SpawnPulse;
            _dash = null;
        }
    }

    private void SpawnPulse(Vector3 startPosition)
    {
        if (_pulseVfx != null)
        {
            _pulseVfx.transform.position = startPosition;
            _pulseVfx.Play();
        }

        Collider[] hits = Physics.OverlapSphere(startPosition, _radius);
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable target)) continue;

            float dmg  = _damage;
            SkillDamageType type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dmg, ref type);
            target.ReceiveDamage(dmg, type);
        }
    }
}
