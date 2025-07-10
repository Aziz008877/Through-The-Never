using UnityEngine;

public class IgneousFlickerPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _reducePercent = 0.20f;
    private PlayerSkillManager _playerSkillManager;
    public override void EnablePassive()
    {
        _playerSkillManager = PlayerContext.PlayerSkillManager;
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled += OnEnemyKilled;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled -= OnEnemyKilled;
    }
    
    private void OnEnemyKilled(Transform killedEnemy)
    {
        float percent = Mathf.Clamp01(_reducePercent);

        foreach (var kv in _playerSkillManager.Actives)
            kv.Value.ReduceCooldownByPercent(percent);
    }
}