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
        
    }

    public virtual void PlayRangedAttack()
    {

    }
}