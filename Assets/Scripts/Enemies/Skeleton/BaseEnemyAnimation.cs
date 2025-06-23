using UnityEngine;
public class BaseEnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _enemyAnimator;
    private readonly int _attack = Animator.StringToHash("Attack");
    private readonly int _isRunning = Animator.StringToHash("IsRunning");
    public void AttackAnimation()
    {
        _enemyAnimator.SetTrigger(_attack);
    }

    public void ChangeMoveState(bool state)
    {
        _enemyAnimator.SetBool(_isRunning, state);
    }
}
