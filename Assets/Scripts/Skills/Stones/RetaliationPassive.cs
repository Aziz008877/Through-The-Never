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

        // сколько отражаем
        float dmgBack = incomingDamage * _returnPercent;

        // формируем контекст отражённого удара
        var ctx = new DamageContext
        {
            Attacker       = Context,
            Target         = attacker,
            SkillBehaviour = this,
            SkillDef       = null,
            Slot           = SkillSlot.Passive,
            Type           = SkillDamageType.Basic,
            Damage         = dmgBack,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = (attacker as Component)?.transform.position ?? Context.transform.position,
            HitNormal      = Vector3.up,
            SourceGO       = gameObject
        };

        Context.ApplyDamageContextModifiers(ref ctx); // применяем контекстные модификаторы
        attacker.ReceiveDamage(ctx);                  // событие "урон нанесён" вызовется внутри цели
    }

}