using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingEnemy : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }

    [Header("UI")]
    [SerializeField] private Image _hpFillImage;
    public bool CanBeDamaged { get; set; } = true;
    public Action<Transform> OnEnemyDead { get; set; }

    private void Awake() => UpdateBar();

    public void ReceiveDamage(float amount, SkillDamageType type)
    {
        if (!CanBeDamaged) return;

        if (type == SkillDamageType.DOT)
            StartCoroutine(DotTick(amount, 3f));
        else
            ApplyDamage(amount);
    }

    private void ApplyDamage(float amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, MinHP);
        UpdateBar();

        if (CurrentHP <= MinHP)
            OnEnemyDead?.Invoke(transform);
    }
    
    private IEnumerator DotTick(float totalDamage, float duration)
    {
        float damagePerSecond = totalDamage / duration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1);
            
            ApplyDamage(damagePerSecond * 1);

            elapsedTime += 1;
        }
    }

    
    private void UpdateBar()
    {
        _hpFillImage.fillAmount = CurrentHP / MaxHP;
    }
}