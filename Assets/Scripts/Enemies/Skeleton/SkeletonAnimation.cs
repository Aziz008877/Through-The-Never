using UnityEngine;

public class SkeletonAnimation : BaseEnemyAnimation
{
    private readonly int _attack = Animator.StringToHash("Attack");
    public override void PlayMeleeAttack()
    {
        _enemyAnimator.SetTrigger(_attack);
    }
}
