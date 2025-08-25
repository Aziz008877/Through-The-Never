using UnityEngine;
public class FreezingWindSkill : PassiveSkillBehaviour
{
    [SerializeField] private float _pushForce;
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
        SpawnIceWind(endPosition, (endPosition - Context.transform.position).normalized);

        Collider[] hits = Physics.OverlapSphere(endPosition, Definition.Raduis);

        foreach (var col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;
            if (col.transform == Context.transform) continue; // опционально: не бьём себя

            var ctx = new DamageContext
            {
                Attacker       = Context,
                Target         = enemy,
                SkillBehaviour = null,                // пассивка
                SkillDef       = Definition,          // чтобы знать, чем нанесено
                Slot           = Definition.Slot,
                Type           = SkillDamageType.Basic,
                Damage         = Definition.Damage,   // здесь твой урон
                IsCrit         = false,               // пассивка без крита (или сам ролльни при желании)
                CritMultiplier = 1f,
                HitPoint       = col.transform.position,
                HitNormal      = Vector3.up,
                SourceGO       = gameObject
            };

            Context.ApplyDamageContextModifiers(ref ctx); // контекстные модификаторы
            enemy.ReceiveDamage(ctx);                     // событие разойдётся внутри цели

            // физический толчок — без изменений
            if (col.attachedRigidbody != null)
            {
                Vector3 dir      = (col.transform.position - endPosition).normalized;
                Vector3 dashDir  = (endPosition - Context.transform.position).normalized;
                Vector3 finalDir = (dir + dashDir * 0.5f).normalized;

                col.attachedRigidbody.linearVelocity = Vector3.zero;
                col.attachedRigidbody.AddForce(finalDir * _pushForce, ForceMode.Impulse);
            }
        }

    }
    private void SpawnIceWind(Vector3 position, Vector3 forward)
    {
        
    }
    
    private void Detach()
    {
        if (!_dash) return;
        _dash.OnDashEnded -= PushStun;
        _dash = null;
    }
}
