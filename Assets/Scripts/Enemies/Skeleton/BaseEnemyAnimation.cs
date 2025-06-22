using UnityEngine;

public class BaseEnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _enemyAnimator;
    private readonly int _attack = Animator.StringToHash("Attack");

    public void AttackAnimation()
    {
        _enemyAnimator.SetTrigger(_attack);
    }
}
