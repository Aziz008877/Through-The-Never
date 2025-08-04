using UnityEngine;

public class IgneousFlickerPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _reducePercent = 0.20f;
    private ISkillManager _playerSkillManager;
    public override void EnablePassive()
    {
        _playerSkillManager = Context.SkillManager;
        Context.EnemyHandler.OnEnemyKilled += OnEnemyKilled;
    }

    public override void DisablePassive()
    {
        Context.EnemyHandler.OnEnemyKilled -= OnEnemyKilled;
    }
    
    private void OnEnemyKilled(Transform killedEnemy)
    {
        float percent = Mathf.Clamp01(_reducePercent);

        foreach (var kv in _playerSkillManager.Actives)
            kv.Value.ReduceCooldownByPercent(percent);
    }
}