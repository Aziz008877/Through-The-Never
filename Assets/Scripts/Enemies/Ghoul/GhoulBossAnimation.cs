using UnityEngine;

public class GhoulBossAnimation : BaseEnemyAnimation
{
    public override void PlayMeleeAttack()
    {
        _enemyAnimator.SetTrigger("MeleeAttack");
    }

    public override void PlayRangedAttack()
    {
        _enemyAnimator.SetTrigger("RangeAttack");
    }
}
