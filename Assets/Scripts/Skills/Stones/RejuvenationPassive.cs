using UnityEngine;

public class RejuvenationPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _bonusHP = 50f;
    [SerializeField] private bool _healToFull;
    public override void EnablePassive()
    {
        PlayerContext.PlayerHp.AddMaxHP(_bonusHP, healToFull: _healToFull);
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerHp.RemoveMaxHP(_bonusHP);
    }
}