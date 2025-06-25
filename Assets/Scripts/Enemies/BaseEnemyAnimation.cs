using UnityEngine;

public class BaseEnemyAnimation : MonoBehaviour
{
    [SerializeField] protected Animator _enemyAnimator;
    protected int _isRunning = Animator.StringToHash("IsRunning");
    public virtual void ChangeMoveState(bool isMoving)
    {
        _enemyAnimator.SetBool(_isRunning, isMoving);
    }
    
    public virtual void PlayMeleeAttack()
    {
        Debug.LogWarning($"{name}: PlayMeleeAttack not implemented.");
    }

    public virtual void PlayRangedAttack()
    {
        Debug.LogWarning($"{name}: PlayRangedAttack not implemented.");
    }
}