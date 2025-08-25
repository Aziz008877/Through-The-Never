using UnityEngine;
public sealed class IfritFervorPassiveUltimate : PassiveSkillBehaviour, IOnDamageDealtContextModifier          
{
    [Header("Fervor parameters")]
    [SerializeField] private float _debuffDuration = 10f;
    [SerializeField] private float _hpThreshold = 0.30f;
    [SerializeField] private GameObject _curseVfx;
    public override void EnablePassive()
    {
        Context.RegisterOnDamageDealtContextModifier(this);
        Debug.Log("<color=orange>[Fervor]</color> passive enabled");
    }

    public override void DisablePassive()
    {
        Context.UnregisterOnDamageDealtContextModifier(this);
    }

    public void OnDamageDealt(in DamageContext ctx)
    {
        if (ctx.Target is not BaseEnemyHP hp) return;
        
        if (!hp.TryGetComponent(out IfritFervorDebuff debuff))
            debuff = hp.gameObject.AddComponent<IfritFervorDebuff>();

        debuff.Init(_debuffDuration, _hpThreshold, _curseVfx);
    }
}