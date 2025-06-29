using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public abstract class BaseEnemyHP : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    public bool CanBeDamaged { get; set; } = true;
    [SerializeField] private Canvas _enemyCanvas;
    [SerializeField] private Image _hpFillValue;
    [SerializeField] private UnityEvent _onEnemyDead;
    [Inject] private DamageTextPool _damageTextPool;

    public Action<Transform> OnEnemyDead { get; set; }

    private Camera _mainCamera;
    private Coroutine _dotCoroutine;

    private void Start()
    {
        _mainCamera = Camera.main;
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
                StopCoroutine(_dotCoroutine);

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

        _hpFillValue.fillAmount = CurrentHP / MaxHP;
    }

    private void Update()
    {
        _enemyCanvas.transform.LookAt(
            _enemyCanvas.transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up
        );
    }

    private void Die()
    {
        OnEnemyDead?.Invoke(transform);
        _onEnemyDead?.Invoke();
        StartCoroutine(DestroySkeleton());
    }

    private IEnumerator DestroySkeleton()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
