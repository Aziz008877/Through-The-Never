using System;
using UnityEngine;

public class SkeletonAnimation : BaseEnemyAnimation
{
    private SkeletonHP _skeletonHp;
    private readonly int _attack = Animator.StringToHash("Attack");
    private readonly int _die = Animator.StringToHash("Die");
    private void Awake()
    {
        _skeletonHp = GetComponent<SkeletonHP>();
        _skeletonHp.OnEnemyDead += Die;
    }

    private void Die(Transform position)
    {
        _enemyAnimator.SetTrigger(_die);
    }

    public override void PlayMeleeAttack()
    {
        _enemyAnimator.SetTrigger(_attack);
    }

    private void OnDestroy()
    {
        _skeletonHp.OnEnemyDead -= Die;
    }
}
