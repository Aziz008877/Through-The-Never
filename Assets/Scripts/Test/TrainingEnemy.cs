using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingEnemy : MonoBehaviour, IDamageable, IDotReceivable
{
    [Header("HP")]
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }

    [Header("UI")]
    [SerializeField] private Image _hpFillImage;

    public bool CanBeDamaged { get; set; } = true;
    public System.Action<Transform> OnEnemyDead { get; set; }

    private Coroutine _dotRoutine;                     // единственная корутина DOT

    private void Awake() => UpdateBar();

    /* ───────────────────────── IDamageable ───────────────────────── */
    public void ReceiveDamage(float amount, SkillDamageType type)
    {
        if (!CanBeDamaged) return;

        if (type == SkillDamageType.DOT)
        {
            // если DOT пришёл как обычный урон, считаем, что это уже тик
            ApplyDamage(amount);
            return;
        }

        ApplyDamage(amount);
    }

    /* ───────────────────────── IDotReceivable ────────────────────── */
    public void ApplyDot(float dps, float duration)
    {
        // перезапускаем эффект, если уже горим
        if (_dotRoutine != null) StopCoroutine(_dotRoutine);
        _dotRoutine = StartCoroutine(DotTick(dps, duration));
    }

    /* ───────────────────────── private helpers ───────────────────── */
    private IEnumerator DotTick(float dps, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1f);       // тик раз в секунду
            ApplyDamage(dps);                          // dps за 1 с
            elapsedTime += 1f;
        }

        _dotRoutine = null;                            // DOT закончился
    }

    private void ApplyDamage(float amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, MinHP);
        UpdateBar();

        if (CurrentHP <= MinHP)
            OnEnemyDead?.Invoke(transform);
    }

    private void UpdateBar() =>
        _hpFillImage.fillAmount = CurrentHP / MaxHP;
}
