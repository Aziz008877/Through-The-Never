using UnityEngine;
public sealed class IfritFervorPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtModifier          
{
    [Header("Fervor parameters")]
    [SerializeField] private float _debuffDuration = 10f;
    [SerializeField] private float _hpThreshold = 0.30f;
    [SerializeField] private GameObject _curseVfx;

    public override void EnablePassive()
    {
        PlayerContext.RegisterOnDamageDealtModifier(this);
        Debug.Log("<color=orange>[Fervor]</color> passive enabled");
    }

    public override void DisablePassive()
    {
        PlayerContext.UnregisterOnDamageDealtModifier(this);
    }
    
    public void OnDamageDealt(IDamageable target, float damage,
        SkillDamageType type, PlayerContext ctx)
    {
        if (target is not BaseEnemyHP hp) return;
        
        if (!hp.TryGetComponent(out IfritFervorDebuff debuff))
            debuff = hp.gameObject.AddComponent<IfritFervorDebuff>();

        debuff.Init(_debuffDuration, _hpThreshold, _curseVfx);
    }
}