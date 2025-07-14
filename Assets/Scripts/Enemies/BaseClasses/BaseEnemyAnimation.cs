using UnityEngine;
public abstract class BaseEnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Animator parameters (optional)")]
    [SerializeField] private string _runBool = "IsRunning";
    [SerializeField] private string _meleeTrigger = "Melee";
    [SerializeField] private string _rangedTrigger = "Ranged";
    [SerializeField] private string _specialTrigger01 = "Special01";
    [SerializeField] private string _specialTrigger02 = "Special02";
    [SerializeField] private string _deathTrigger = "Die";
    
    private int _runHash = Animator.StringToHash("IsRunning");
    private int _meleeHash = -1;
    private int _rangedHash = -1;
    private int _specialHash01 = -1;
    private int _specialHash02 = -1;
    private int _deathHash = -1;

    protected virtual void Awake()
    {
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        
        if (!string.IsNullOrEmpty(_runBool)) _runHash = Animator.StringToHash(_runBool);
        if (!string.IsNullOrEmpty(_meleeTrigger)) _meleeHash = Animator.StringToHash(_meleeTrigger);
        if (!string.IsNullOrEmpty(_rangedTrigger)) _rangedHash = Animator.StringToHash(_rangedTrigger);
        if (!string.IsNullOrEmpty(_specialTrigger01)) _specialHash01 = Animator.StringToHash(_specialTrigger01);
        if (!string.IsNullOrEmpty(_specialTrigger02)) _specialHash02 = Animator.StringToHash(_specialTrigger02);
        if (!string.IsNullOrEmpty(_deathTrigger)) _deathHash = Animator.StringToHash(_deathTrigger);
    }

    public virtual void SetMove(bool isMoving)
    {
        if (_animator && _runHash != -1)
        {
            _animator.SetBool(_runHash, isMoving);
        }
    }

    public virtual void PlayMeleeAttack()
    {
        PlayTrigger(_meleeHash);
    }

    public virtual void PlayRangedAttack()
    {
        PlayTrigger(_rangedHash);
    }
    
    public virtual void PlaySpecial01()
    {
        PlayTrigger(_specialHash01);
    }
    
    public virtual void PlaySpecial02()
    {
        PlayTrigger(_specialHash02);
    }

    public virtual void PlayDeath()
    {
        PlayTrigger(_deathHash);
    }

    protected void PlayTrigger(int hash)
    {
        if (_animator && hash != -1)
            _animator.SetTrigger(hash);
    }
    
    public virtual void MeleeHitEvent()   { }

    public virtual void RangedFireEvent() { }

    public virtual void Special01Event()  { }

    public virtual void Special02Event()  { }
}

public enum EnemyAnimAction
{
    Melee,
    Ranged,
    Special01,
    Special02
}
