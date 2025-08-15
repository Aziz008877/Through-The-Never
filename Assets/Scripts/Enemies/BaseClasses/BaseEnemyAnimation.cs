using UnityEngine;

public abstract class BaseEnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [Header("Animator parameters (optional)")]
    [SerializeField] private string _runBool = "IsRunning";
    [SerializeField] private string _meleeTrigger = "Melee";
    [SerializeField] private string _rangedTrigger = "Ranged";
    [SerializeField] private string _special01Trig = "Special01";
    [SerializeField] private string _special02Trig = "Special02";
    [SerializeField] private string _deathTrigger = "Die";
    [SerializeField] private string _stunBool = "IsStunned";
    private int _runHash, _meleeHash, _rangedHash, _spec01Hash, _spec02Hash, _deathHash;
    protected virtual void Awake()
    {
        _runHash = Animator.StringToHash(_runBool);
        _meleeHash = Animator.StringToHash(_meleeTrigger);
        _rangedHash = Animator.StringToHash(_rangedTrigger);
        _spec01Hash = Animator.StringToHash(_special01Trig);
        _spec02Hash = Animator.StringToHash(_special02Trig);
        _deathHash = Animator.StringToHash(_deathTrigger);
    }
    
    public virtual void SetMove(bool moving) => _animator.SetBool(_runHash, moving);
    public virtual void PlayMeleeAttack() => PlayTrigger(_meleeHash);
    public virtual void PlayRangedAttack() => PlayTrigger(_rangedHash);
    public virtual void PlaySpecial01() => PlayTrigger(_spec01Hash);
    public virtual void PlaySpecial02() => PlayTrigger(_spec02Hash);

    public virtual void Stun(bool state)
    {
        _animator.SetBool(_stunBool, state);
    }
    public virtual void PlayDeath() => PlayTrigger(_deathHash);

    private void PlayTrigger(int hash)
    {
        if (_animator && hash != 0) _animator.SetTrigger(hash);
    }

    public void MeleeHitEvent()
    {
        GetComponent<BaseEnemyAttack>().HandleMeleeHit();
    }
    public void RangedFireEvent() => GetComponent<BaseEnemyAttack>()?.HandleRangedFire();
    public void MeleeEndEvent() => GetComponent<BaseEnemyAttack>()?.MeleeEndEvent();
    public void RangedEndEvent() => GetComponent<BaseEnemyAttack>()?.RangedEndEvent();
    public virtual void Special01Event() { }
    public virtual void Special02Event() { }
}
