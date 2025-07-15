/*using UnityEngine;
public sealed class IfritFervorUltimate : PassiveSkillBehaviour, IOnDamageDealtModifier
{
    [SerializeField] float _hpThreshold = .30f;
    [SerializeField] float _frenzyTime  = 8f;
    [SerializeField] float _dmgBonusPct = .25f;
    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
    }

    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
    }
    
    public void OnDamageDealt(IDamageable target, float damage, SkillDamageType type, PlayerContext ctx)
    {
        if (target is not MonoBehaviour mb) return;
        if (mb.TryGetComponent<IfritFervorStatus>(out _)) return;

        var st = mb.gameObject.AddComponent<IfritFervorStatus>();
        st.Init(_hpThreshold, _frenzyTime, _dmgBonusPct, ctx);
    }
}*/