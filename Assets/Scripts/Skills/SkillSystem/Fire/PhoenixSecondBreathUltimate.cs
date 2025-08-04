using UnityEngine;
public sealed class PhoenixSecondBreathUltimate : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [Header("Heal settings")]
    [SerializeField, Range(0f, 1f)]
    private float _healPercent = 0.25f;
    [SerializeField] private float _maxHealPerHit = 0f;
    [SerializeField] private ParticleSystem _healVfx;
    public override void EnablePassive()
    {
        Context.RegisterOnDamageDealtModifier(this);
        Debug.Log("<color=orange>[Second Breath]</color> enabled");
    }

    public override void DisablePassive()
    {
        Context.UnregisterOnDamageDealtModifier(this);
        Debug.Log("<color=orange>[Second Breath]</color> disabled");
    }

    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, ActorContext ctx)
    {
        if (damage <= 0f) return;

        float heal = damage * _healPercent;
        if (_maxHealPerHit > 0f) heal = Mathf.Min(heal, _maxHealPerHit);

        Context.Hp.ReceiveHP(heal);

        if (_healVfx)
        {
            _healVfx.transform.position = Context.ActorPosition.position;
            _healVfx.Play(true);
        }

        Debug.Log(
            $"<color=orange>[Second Breath]</color> healed {heal:F1} HP " +
            $"({ _healPercent:P0} of {damage:F1})");
    }
}
