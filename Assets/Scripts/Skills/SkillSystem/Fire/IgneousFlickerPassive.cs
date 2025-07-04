using UnityEngine;
public class IgneousFlickerPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _reducePercent = 0.20f;
    private PlayerSkillManager _skillManager;
    public override void EnablePassive()
    {
        _skillManager = PlayerContext.GetComponent<PlayerSkillManager>();
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled += HandleKill;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerEnemyHandler.OnEnemyKilled -= HandleKill;
    }

    private void HandleKill(Transform killedEnemy)
    {
        float percent = Mathf.Clamp01(_reducePercent);

        foreach (var pair in _skillManager.Actives)
        {
            Debug.Log(pair.Value.Definition.Id);
            Debug.Log(pair.Value.CoolDownTimer);
            pair.Value.ReduceCooldownByPercent(percent);
            Debug.Log(pair.Value.CoolDownTimer);
        }
    }
}