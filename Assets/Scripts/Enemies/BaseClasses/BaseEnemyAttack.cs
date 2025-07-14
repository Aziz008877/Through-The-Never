using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(BaseEnemyAnimation))]
public abstract class BaseEnemyAttack : MonoBehaviour
{
    [Header("Melee")]
    [SerializeField] private bool  _useMelee = true;
    [SerializeField] private float _meleeDamage = 10f;
    [SerializeField] private float _meleeDist = 2f;
    [SerializeField] private float _meleeCd = 2f;

    [Header("Ranged")]
    [SerializeField] private bool  _useRanged = true;
    [SerializeField] private float _rangedDamage = 8f;
    [SerializeField] private float _rangedDist = 6f;
    [SerializeField] private float _rangedCd = 4f;

    [Header("Behaviour")]
    [SerializeField] private float _stopAfterCast = .8f;
    
    private Transform _target;
    private BaseEnemyAnimation _anim;

    private float _meleeTimer;
    private float _rangedTimer;
    private float _lockTimer;
    public event Action<float> OnAttackStarted;
    public bool  IsCasting { get; private set; }
    
    private void Awake() => _anim = GetComponent<BaseEnemyAnimation>();

    private void Update()
    {
        if (_target == null) return;

        TickTimers();

        if (_lockTimer > 0f) return;

        float dist = Vector3.Distance(transform.position, _target.position);

        if (_useMelee && _meleeTimer <= 0f && dist <= _meleeDist)
        {
            StartCoroutine(CoMelee());
            return;
        }

        if (_useRanged && _rangedTimer <= 0f && dist <= _rangedDist)
        {
            StartCoroutine(CoRanged());
        }
    }
    public void SetTarget(Transform t) => _target = t;
    public bool ShouldStopMovement() => _lockTimer > 0f;
    
    private IEnumerator CoMelee()
    {
        BeginCast();
        _anim.PlayMeleeAttack();
        yield return null;
        _meleeTimer = _meleeCd;
        _lockTimer = _stopAfterCast;
        EndCast();
    }

    private IEnumerator CoRanged()
    {
        BeginCast();
        _anim.PlayRangedAttack();
        yield return null;
        _rangedTimer = _rangedCd;
        _lockTimer = _stopAfterCast;
        EndCast();
    }
    public void MeleeHitEvent()   => DoMeleeHit();
    public void RangedFireEvent() => DoRangedShot();

    protected virtual void DoMeleeHit()
    {
        if (!CanHitTarget(_meleeDamage, out var hp)) return;
        hp.ReceiveDamage(_meleeDamage);
    }

    protected virtual void DoRangedShot()
    {
        if (!CanHitTarget(_rangedDamage, out var hp)) return;
        hp.ReceiveDamage(_rangedDamage);
    }
    
    private bool CanHitTarget(float dmg, out PlayerHP hp)
    {
        hp = null;
        if (_target == null) return false;
        
        hp = _target.GetComponent<PlayerHP>();
        if (hp == null) return false;
        
        if (_target.TryGetComponent<IBlindable>(out var blind) && blind.IsBlinded())
        {
            float missChance = blind.CurrentMissChance;
            if (UnityEngine.Random.value < missChance)
                return false;
        }
        return true;
    }

    private void TickTimers()
    {
        if (_meleeTimer > 0f) _meleeTimer -= Time.deltaTime;
        if (_rangedTimer > 0f) _rangedTimer -= Time.deltaTime;
        if (_lockTimer > 0f) _lockTimer -= Time.deltaTime;
    }

    private void BeginCast()
    {
        IsCasting = true;
        OnAttackStarted?.Invoke(_stopAfterCast);
    }

    private void EndCast()
    {
        IsCasting = false;
    }
}
