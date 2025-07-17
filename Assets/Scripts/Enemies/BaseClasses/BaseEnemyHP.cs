using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
public abstract class BaseEnemyHP : MonoBehaviour, IDamageable, IDotReceivable
{
    [Header("HP Data")]
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    public bool IsDotActive { get; set; }
    public bool CanBeDamaged { get; set; } = true;
    [SerializeField] private UnityEvent _onEnemyDead;
    [Inject] private DamageTextPool _damageTextPool;
    public Action<Transform> OnEnemyDead { get; set; }
    private Coroutine _dotCoroutine;
    private BaseEnemyAnimation _enemyAnimation;
    private BaseEnemyMove _enemyMove;

    private void Start()
    {
        _enemyAnimation = GetComponent<BaseEnemyAnimation>();
        _enemyMove = GetComponent<BaseEnemyMove>();
    }

    public void Init(DamageTextPool damageTextPool)
    {
        _damageTextPool = damageTextPool;
    }

    public void ReceiveDamage(float damageValue, SkillDamageType type)
    {
        if (type == SkillDamageType.DOT)
        {
            if (_dotCoroutine != null)
            {
                StopCoroutine(_dotCoroutine);
            }
            
            _dotCoroutine = StartCoroutine(ApplyDot(damageValue, 3, 1f));
            return;
        }

        ApplyDamage(damageValue);
    }

    private IEnumerator ApplyDot(float damagePerTick, float duration, float tickRate)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ApplyDamage(damagePerTick);
            yield return new WaitForSeconds(1f / tickRate);
            elapsed += 1f / tickRate;
        }
    }

    private void ApplyDamage(float damageValue)
    {
        if (!CanBeDamaged) return;
        
        if (CurrentHP - damageValue <= MinHP)
        {
            CurrentHP = MinHP;
            CanBeDamaged = false;
            Die();
        }
        else
        {
            CurrentHP -= damageValue;
            damageValue = Mathf.Round(damageValue * 10f) / 10f;
            _damageTextPool.ShowDamage(damageValue, transform.position);
        }
    }

    private void Die()
    {
        OnEnemyDead?.Invoke(transform);
        _onEnemyDead?.Invoke();
        _enemyAnimation.PlayDeath();
        _enemyMove.StopChasing();
        StartCoroutine(DestroySkeleton());
    }

    private IEnumerator DestroySkeleton()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void ApplyDot(float dps, float duration)
    {
        
    }

    public void RefreshDot(float duration)
    {
        
    }
}
