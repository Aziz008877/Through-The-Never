using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
public abstract class BaseEnemyHP : MonoBehaviour, IDamageable, IDotReceivable
{
    [Header("Reward")]
    [SerializeField] private int _coinReward = 500;
    [field: SerializeField] public float CurrentHP { get; set; }
    [field: SerializeField] public float MinHP { get; set; }
    [field: SerializeField] public float MaxHP { get; set; }
    public bool CanBeDamaged { get; set; } = true;
    public bool IsDotActive  { get; set; }
    [SerializeField] private UnityEvent _onEnemyDead;
    //[Inject] private DamageTextPool _damageTextPool;
    public event Action<DamageContext> OnDamaged;
    public event Action<DamageContext> OnKilled;
    private BaseEnemyAnimation _enemyAnimation;
    private BaseEnemyMove _enemyMove;
    private Coroutine _dotCoroutine;
    private float _dotDps;
    private float _dotTickRate = 1f;
    public event Action<Transform> OnEnemyDead;
    private void Start()
    {
        _enemyAnimation = GetComponent<BaseEnemyAnimation>();
        _enemyMove      = GetComponent<BaseEnemyMove>();
    }

    public void ReceiveDamage(in DamageContext ctx)
    {
        if (!CanBeDamaged) return;

        if (ctx.HasDot || ctx.Type == SkillDamageType.DOT)
        {
            ApplyDot(ctx.DotDps, ctx.DotDuration, ctx.DotTickRate <= 0f ? 1f : ctx.DotTickRate);
            return;
        }

        ApplyDamageInternal(ctx);
    }

    private void ApplyDamageInternal(in DamageContext ctx)
    {
        if (!CanBeDamaged) return;

        float dmg = Mathf.Max(0f, ctx.Damage);

        if (CurrentHP - dmg <= MinHP)
        {
            CurrentHP    = MinHP;
            CanBeDamaged = false;

            //if (dmg > 0f) _damageTextPool.ShowDamage(Mathf.Round(dmg * 10f) / 10f, transform.position);

            OnDamaged?.Invoke(ctx);
            Die(ctx);
        }
        else
        {
            CurrentHP -= dmg;
            //if (dmg > 0f) _damageTextPool.ShowDamage(Mathf.Round(dmg * 10f) / 10f, transform.position);

            OnDamaged?.Invoke(ctx);
        }
    }

    private void Die(in DamageContext lastCtx)
    {
        var meta = MetaProgressionService.Instance;
        if (meta) meta.AddCoins(_coinReward);
        
        OnEnemyDead?.Invoke(transform);
        OnKilled?.Invoke(lastCtx);
        _onEnemyDead?.Invoke();
        _enemyAnimation.PlayDeath();
        _enemyMove.StopChasing();
        StartCoroutine(DestroyGameObject());
        EnemyDeathReporter.ReportEnemyDied(transform);
    }

    private IEnumerator DestroyGameObject()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void ApplyDot(float dps, float duration, float tickRate = 1f)
    {
        _dotDps = dps;
        _dotTickRate = Mathf.Max(0.01f, tickRate);
        RefreshDot(duration);
    }

    public void RefreshDot(float duration)
    {
        if (duration <= 0f) return;
        if (_dotCoroutine != null) StopCoroutine(_dotCoroutine);
        _dotCoroutine = StartCoroutine(DotRoutine(duration));
        IsDotActive = true;
    }

    private IEnumerator DotRoutine(float duration)
    {
        float elapsed = 0f;
        var wait = new WaitForSeconds(1f / _dotTickRate);

        while (elapsed < duration)
        {
            var tickCtx = new DamageContext
            {
                Attacker = null,
                Target = this,
                SkillBehaviour = null,
                SkillDef = null,
                Slot = SkillSlot.Undefined,
                Type = SkillDamageType.DOT,
                Damage = _dotDps,
                IsCrit = false,
                CritMultiplier = 1f
            };

            ApplyDamageInternal(tickCtx);

            yield return wait;
            elapsed += 1f / _dotTickRate;
        }

        IsDotActive  = false;
        _dotCoroutine = null;
    }
}
