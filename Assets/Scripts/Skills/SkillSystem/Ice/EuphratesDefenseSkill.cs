using System;
using System.Collections;
using UnityEngine;

public class EuphratesDefenseSkill : ActiveSkillBehaviour, IDefenceDurationSkill
{
    [Header("Core")]
    [SerializeField] private float _duration = 5f;       // время сферы (иммун)
    [SerializeField] private float _cooldown = 15f;      // если КД хранится в Definition – убери это поле

    [Header("Regeneration")]
    [SerializeField] private float _regenSeconds = 10f;          // время регена
    [SerializeField] private float _regenPercentOfMaxHP = 0.30f; // 30% MaxHP за всё окно
    [SerializeField] private bool  _regenStartsImmediately = true;

    [Header("Damage Bonus From Absorbed")]
    [SerializeField] private float _bonusDuration = 6f;
    [SerializeField] private float _bonusPer100Absorbed = 0.10f; // +10% за каждые 100 поглощённого
    [SerializeField] private float _bonusCapPercent = 0.60f;     // максимум +60%

    [Header("VFX / SFX")]
    [SerializeField] private ParticleSystem _bubbleVfx;

    private float _absorbedTotal;
    private Coroutine _defRoutine;
    private Coroutine _regenRoutine;

    // временный модификатор исходящего урона
    private BonusDamageMod _bonusMod;

    public event Action OnDefenceStarted;
    public event Action OnDefenceFinished;

    public override void TryCast()
    {
        if (!IsReady || _defRoutine != null) return;

        StartCooldown();

        _absorbedTotal = 0f;

        if (_regenStartsImmediately)
            StartRegen();

        _defRoutine = StartCoroutine(DefenseRoutine());
    }

    private IEnumerator DefenseRoutine()
    {
        if (_bubbleVfx) _bubbleVfx.Play();

        Context.Hp.OnIncomingDamage += OnIncomingDamage;
        Context.State.ChangePlayerState(false);
        OnDefenceStarted?.Invoke();

        float t = 0f;
        while (t < _duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        OnDefenceFinished?.Invoke();
        CleanupDefense();

        GrantDamageBonusFromAbsorbed(); // выдаём бафф урона после окончания сферы

        if (!_regenStartsImmediately)
            StartRegen();

        _defRoutine = null;
    }

    private void OnIncomingDamage(ref float damage, IDamageable source)
    {
        if (damage <= 0f) return;

        _absorbedTotal += damage;
        damage = 0f; // полный игнор урона пока длится защита
    }

    private void StartRegen()
    {
        if (_regenSeconds <= 0f || _regenPercentOfMaxHP <= 0f) return;
        if (_regenRoutine != null) StopCoroutine(_regenRoutine);
        _regenRoutine = StartCoroutine(RegenerateRoutine());
    }

    private IEnumerator RegenerateRoutine()
    {
        float totalHeal = Context.Hp.MaxHP * Mathf.Max(0f, _regenPercentOfMaxHP);
        float left = totalHeal;
        float timeLeft = Mathf.Max(0.01f, _regenSeconds);

        while (timeLeft > 0f && left > 0f)
        {
            float dt = Time.deltaTime;
            float chunk = totalHeal * (dt / _regenSeconds);
            chunk = Mathf.Min(chunk, left);
            left -= chunk;
            timeLeft -= dt;

            if (chunk > 0f)
                Context.Hp.ReceiveHP(chunk);

            yield return null;
        }

        _regenRoutine = null;
    }

    private void GrantDamageBonusFromAbsorbed()
    {
        if (_absorbedTotal <= 0f || _bonusDuration <= 0f) return;

        float bonusPct = (_absorbedTotal / 100f) * Mathf.Max(0f, _bonusPer100Absorbed);
        float cap = Mathf.Max(0f, _bonusCapPercent);
        if (cap > 0f) bonusPct = Mathf.Min(bonusPct, cap);

        if (bonusPct <= 0f) return;

        // создаём и регистрируем временный контекст-модификатор урона
        RemoveBonusModIfAny(); // зачистим предыдущий, если вдруг был
        _bonusMod = new BonusDamageMod(bonusPct, Time.time + _bonusDuration);
        Context.RegisterContextModifier(_bonusMod);
        StartCoroutine(BonusLifetime(_bonusDuration));
    }

    private IEnumerator BonusLifetime(float dur)
    {
        float t = dur;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        RemoveBonusModIfAny();
    }

    private void RemoveBonusModIfAny()
    {
        if (_bonusMod != null)
        {
            Context.UnregisterContextModifier(_bonusMod);
            _bonusMod = null;
        }
    }

    private void CleanupDefense()
    {
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;
        Context.State.ChangePlayerState(true);
        if (_bubbleVfx) _bubbleVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnDisable()
    {
        if (_defRoutine != null)
        {
            StopCoroutine(_defRoutine);
            _defRoutine = null;
        }
        if (_regenRoutine != null)
        {
            StopCoroutine(_regenRoutine);
            _regenRoutine = null;
        }
        RemoveBonusModIfAny();
        CleanupDefense();
    }

    public float GetDefenceDuration() => _duration;
    
    private sealed class BonusDamageMod : IDamageContextModifier
    {
        private readonly float _mult;     // 1 + бонус
        private readonly float _expireAt; // Time.time, когда истекает

        public BonusDamageMod(float bonusPercent, float expireAt)
        {
            _mult = 1f + Mathf.Max(0f, bonusPercent); // bonusPercent = 0.25 => mult = 1.25
            _expireAt = expireAt;
        }

        public void Apply(ref DamageContext ctx)
        {
            if (Time.time >= _expireAt) return;
            // Увеличиваем исходящий урон игрока
            ctx.Damage *= _mult;
        }
    }
}
