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

        foreach (Collider col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;
            
            float dmg = Definition.Damage;
            SkillDamageType type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dmg, ref type);
            enemy.ReceiveDamage(dmg, type);
            Context.FireOnDamageDealt(enemy, dmg, type);

            if (col.attachedRigidbody != null)
            {
                Vector3 dir = (col.transform.position - endPosition).normalized;
                Vector3 dashDir = (endPosition - Context.transform.position).normalized;
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
