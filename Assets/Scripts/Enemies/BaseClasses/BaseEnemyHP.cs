using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class BaseEnemyHP : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    [SerializeField] private Canvas _enemyCanvas;
    [SerializeField] private Image _hpFillValue;
    [Inject] private DamageTextPool _damageTextPool;
    public Action<Transform> OnEnemyDead { get; set; }
    private Camera _mainCamera;
    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void ReceiveDamage(float damageValue, SkillDamageType type)
    {
        if (CurrentHP - damageValue <= MinHP)
        {
            Die();
        }
        else
        {
            CurrentHP -= damageValue;
            _damageTextPool.ShowDamage(damageValue, transform.position);
        }

        _hpFillValue.fillAmount = CurrentHP / MaxHP;
    }

    private void Update()
    {
        _enemyCanvas.transform.LookAt(_enemyCanvas.transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
    }

    private void Die()
    {
        OnEnemyDead?.Invoke(transform);
        Destroy(gameObject);
    }
}
