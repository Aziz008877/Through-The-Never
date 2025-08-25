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

        var hits = Physics.OverlapSphere(startPosition, _radius);
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!h.TryGetComponent(out IDamageable target)) continue;
            if (h.transform == Context.transform) continue; // не бьём себя (по желанию)

            var ctx = new DamageContext
            {
                Attacker       = Context,
                Target         = target,
                SkillBehaviour = null,              // это не ActiveSkillBehaviour
                SkillDef       = Definition,        // если есть Definition; иначе = null
                Slot           = Definition ? Definition.Slot : SkillSlot.Undefined,
                Type           = SkillDamageType.Basic,
                Damage         = _damage,
                IsCrit         = false,
                CritMultiplier = 1f,
                HitPoint       = h.transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            Context.ApplyDamageContextModifiers(ref ctx);
            target.ReceiveDamage(ctx);              // события разойдутся внутри цели
        }
    }

}
