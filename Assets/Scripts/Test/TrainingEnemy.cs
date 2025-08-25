using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingEnemy : MonoBehaviour, IDamageable, IDotReceivable
{
    [Header("HP")]
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    [field: SerializeField] public bool IsDotActive { get; set; }

    [Header("UI")]
    [SerializeField] private Image _hpFillImage;

    public bool CanBeDamaged { get; set; } = true;
    private Coroutine _dotRoutine;
    public event Action<Transform> OnEnemyDead;
    public void ReceiveDamage(in DamageContext ctx)
    {
        throw new System.NotImplementedException();
    }
    private void Awake() => UpdateBar();
    public void ReceiveDamage(float amount, SkillDamageType type)
    {
        if (!CanBeDamaged) return;
        Debug.Log(amount);
        if (type == SkillDamageType.DOT)
        {
            ApplyDamage(amount);
            return;
        }

        ApplyDamage(amount);
    }

    public void ApplyDot(float dps, float duration)
    {
        if (_dotRoutine != null) StopCoroutine(_dotRoutine);
        _dotRoutine = StartCoroutine(DotTick(dps, duration));
    }

    public void ApplyDot(float dps, float duration, float tickRate = 1)
    {
        throw new System.NotImplementedException();
    }

    public void RefreshDot(float duration)
    {
        
    }

    private IEnumerator DotTick(float dps, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration && CanBeDamaged)
        {
            yield return new WaitForSeconds(1f);
            ApplyDamage(dps);
            elapsedTime += 1f;
        }

        _dotRoutine = null;
    }

    private void ApplyDamage(float amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, MinHP);
        UpdateBar();

        if (CurrentHP <= MinHP)
        {
            CanBeDamaged = false;
            OnEnemyDead?.Invoke(transform);
        }
    }

    private void UpdateBar() =>
        _hpFillImage.fillAmount = Mathf.Clamp01(CurrentHP / MaxHP);
}
