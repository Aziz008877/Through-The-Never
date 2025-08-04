using UnityEngine;

public class RejuvenationPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _bonusHP = 50f;
    [SerializeField] private bool _healToFull;
    public override void EnablePassive()
    {
        Context.Hp.AddMaxHP(_bonusHP, healToFull: _healToFull);
    }

    public override void DisablePassive()
    {
        Context.Hp.RemoveMaxHP(_bonusHP);
    }
}