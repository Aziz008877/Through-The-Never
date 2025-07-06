using UnityEngine;
public class IgneousFlickerPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _reducePercent = 0.20f;
    private PlayerSkillManager _skillManager;
    public override void EnablePassive()
    {
        _skillManager = PlayerContext.PlayerSkillManager;
        PlayerContext.SkillModifierHub.Register(this);
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled += HandleKill;
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled -= HandleKill;
    }

    private void HandleKill(Transform killedEnemy)
    {
        float percent = Mathf.Clamp01(_reducePercent);

        foreach (var pair in _skillManager.Actives)
        {
            pair.Value.ReduceCooldownByPercent(percent);
        }
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (key.Stat == SkillStat.Cooldown)
        {
            currentValue -= currentValue * _reducePercent;
            return currentValue;
        }
            
        return currentValue;
    }
}