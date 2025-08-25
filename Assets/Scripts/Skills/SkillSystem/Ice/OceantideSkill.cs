using UnityEngine;
public class OceantideSkill : PassiveSkillBehaviour
{
    [SerializeField] private float _pushForce;
    [SerializeField] private float _stunDuration;
    private ISkillManager _skillManager;
    private PlayerDashSkill _dash;
    public override void EnablePassive()
    {
        _skillManager = Context.SkillManager;
        AttachToDash(_skillManager.GetActive(SkillSlot.Dash));
        _skillManager.ActiveRegistered += OnActiveRegistered;
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash)
        {
            AttachToDash(beh);
        }
    }

    private void AttachToDash(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashEnded += PushStun;
        }
    }

    public override void DisablePassive()
    {
        _skillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void PushStun(Vector3 endPosition)
    {
        Collider[] hits = Physics.OverlapSphere(Context.transform.position, Definition.Raduis);

        foreach (var col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;
            if (col.transform == Context.transform) continue; // опционально: не бьём себя

            var ctx = new DamageContext
            {
                Attacker       = Context,
                Target         = enemy,
                SkillBehaviour = this,                 // теперь поле базового типа SkillBehaviour
                SkillDef       = Definition,
                Slot           = Definition.Slot,
                Type           = SkillDamageType.Basic,
                Damage         = Definition.Damage,    // урон пассивки
                IsCrit         = false,                // при желании можешь зароллить крит вручную
                CritMultiplier = Context.CritMultiplier,
                HitPoint       = col.transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            Context.ApplyDamageContextModifiers(ref ctx);
            enemy.ReceiveDamage(ctx);                  // события разойдутся из цели автоматически

            if (col.TryGetComponent(out StunDebuff stun))
                stun.ApplyStun(_stunDuration);

            if (col.attachedRigidbody != null)
            {
                Vector3 dir = (col.transform.position - Context.transform.position).normalized;
                col.attachedRigidbody.AddForce(dir * _pushForce, ForceMode.VelocityChange);
            }
        }

    }
    
    private void Detach()
    {
        if (!_dash) return;
        _dash.OnDashEnded -= PushStun;
        _dash = null;
    }
}
