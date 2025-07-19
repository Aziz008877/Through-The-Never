using UnityEngine;
public sealed class SunstrikePassive : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] [Range(0f, 2f)] private float _bonusPercent = 0.25f;
    [SerializeField] private float _igniteDuration = 5f;
    private bool _applyingBonus;
    private float BonusMul => 1f + Mathf.Max(0f, _bonusPercent);

    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
        Debug.Log("<color=yellow>[Sunstrike]</color> enabled");
    }

    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
        Debug.Log("<color=yellow>[Sunstrike]</color> disabled");
    }

    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, PlayerContext ctx)
    {
        if (_applyingBonus) return;
        
        if (target is not IDotReceivable dot || !dot.IsDotActive)
        {
            Debug.Log("<color=yellow>[Sunstrike]</color> no Ignite – skip");
            return;
        }

        float extra = damage * (BonusMul - 1f);
        
        if (extra <= 0f)
        {
            Debug.Log("<color=yellow>[Sunstrike]</color> extra ≤ 0 – skip");
            return;
        }

        _applyingBonus = true;
        target.ReceiveDamage(extra, type);
        dot.RefreshDot(_igniteDuration);
        _applyingBonus = false;

        Debug.Log(
            $"<color=yellow>[Sunstrike]</color> +{_bonusPercent:P0} " +
            $"({extra:F1}) extra dmg, Ignite → {_igniteDuration:F1}s");
    }
}
