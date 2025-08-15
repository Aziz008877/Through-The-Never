using UnityEngine;
public class OceantidePassive : PassiveSkillBehaviour
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

        foreach (Collider col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;
        
            float dmg  = Definition.Damage;
            SkillDamageType type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dmg, ref type);
            enemy.ReceiveDamage(dmg, type);
            Context.FireOnDamageDealt(enemy, dmg, type);
            
            if (col.TryGetComponent(out StunDebuff stun))
            {
                stun.ApplyStun(_stunDuration);
            }

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
