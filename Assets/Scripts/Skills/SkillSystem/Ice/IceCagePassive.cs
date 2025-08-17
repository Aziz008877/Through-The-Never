using UnityEngine;

public class IceCagePassive : PassiveSkillBehaviour
{
    [SerializeField] private IceCage _iceCage;
    private ISkillManager _playerSkillManager;
    public override void EnablePassive()
    {
        _playerSkillManager = Context.SkillManager;
        Context.EnemyHandler.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnEnemyKilled(Transform killedEnemy)
    {
        Instantiate(_iceCage, killedEnemy.position, Quaternion.identity);
    }
    
    public override void DisablePassive()
    {
        Context.EnemyHandler.OnEnemyKilled -= OnEnemyKilled;
    }
}
