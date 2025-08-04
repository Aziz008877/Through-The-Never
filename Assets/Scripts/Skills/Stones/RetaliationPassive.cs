using UnityEngine;
public class RetaliationPassive : PassiveSkillBehaviour
{
    [SerializeField, Range(0f,1f)] private float _returnPercent = 0.25f;

    public override void EnablePassive() =>
        Context.Hp.OnActorReceivedDamage += OnPlayerHit;

    public override void DisablePassive() =>
        Context.Hp.OnActorReceivedDamage -= OnPlayerHit;

    private void OnPlayerHit(float incomingDamage, IDamageable attacker)
    {
        if (incomingDamage <= 0f || attacker == null) return;

        float dmgBack = incomingDamage * _returnPercent;
        SkillDamageType type = SkillDamageType.Basic;
        Context.ApplyDamageModifiers(ref dmgBack, ref type);

        attacker.ReceiveDamage(dmgBack, type);
        Context.FireOnDamageDealt(attacker, dmgBack, type);
    }
}