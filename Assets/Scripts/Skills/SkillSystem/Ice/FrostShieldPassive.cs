using System.Collections;
using UnityEngine;

public sealed class FrostShieldPassive : PassiveSkillBehaviour
{
    [Header("Reflect while Dash is active")]
    [SerializeField, Range(0f, 1f)] private float _reflectPercent = 1f; // 1 = 100%

    private ActiveSkillBehaviour _dash;
    private PlayerHP _playerHp;
    private Coroutine _window;
    private float _reflectUntil = -1f;

    public override void EnablePassive()
    {
        _playerHp = Context.Hp as PlayerHP;                 // нужен хук входящего урона
        HookDash(Context.SkillManager.GetActive(SkillSlot.Dash));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;

        if (_playerHp != null)
            _playerHp.OnIncomingDamage += OnIncomingDamage; // ref float dmg, IDamageable source
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        UnhookDash();
        if (_playerHp != null)
            _playerHp.OnIncomingDamage -= OnIncomingDamage;

        if (_window != null) { StopCoroutine(_window); _window = null; }
        _reflectUntil = -1f;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) HookDash(beh);
    }

    private void HookDash(ActiveSkillBehaviour dash)
    {
        UnhookDash();
        if (!dash) return;
        _dash = dash;
        // base.TryCast() внутри даша вызывает OnSkillActivated(Duration)
        _dash.OnSkillActivated += OnDashActivated;
    }

    private void UnhookDash()
    {
        if (_dash != null)
        {
            _dash.OnSkillActivated -= OnDashActivated;
            _dash = null;
        }
    }

    private void OnDashActivated(float duration)
    {
        _reflectUntil = Time.time + Mathf.Max(0f, duration);
        if (_window != null) StopCoroutine(_window);
        _window = StartCoroutine(CloseWindowAfter(duration));
        // Debug.Log($"[FrostShield] window {duration:F2}s");
    }

    private IEnumerator CloseWindowAfter(float duration)
    {
        if (duration > 0f) yield return new WaitForSeconds(duration);
        _reflectUntil = -1f;
        _window = null;
    }

    // Вызывается из PlayerHP перед применением урона
    private void OnIncomingDamage(ref float dmg, IDamageable source)
    {
        if (Time.time > _reflectUntil) return;
        if (source == null || dmg <= 0f) return;

        float reflected = dmg * _reflectPercent;

        // собираем DamageContext так, чтобы килл считался как "Dash"
        var comp = source as Component;
        var ctx = new DamageContext
        {
            Attacker       = Context,
            Target         = source,
            SkillBehaviour = (SkillBehaviour)_dash ?? this, // поведение: даш, если доступен
            SkillDef       = _dash ? _dash.Definition : Definition,
            Slot           = SkillSlot.Dash,                // критично для "килл дашем"
            Type           = SkillDamageType.Basic,
            Damage         = reflected,
            IsCrit         = false,
            CritMultiplier = 1f,
            HitPoint       = comp ? comp.transform.position : Context.transform.position,
            HitNormal      = Vector3.up,
            SourceGO       = gameObject
        };

        Context.ApplyDamageContextModifiers(ref ctx); // стандартные исходящие моды
        source.ReceiveDamage(ctx);

        // Debug.Log($"[FrostShield] reflect {dmg:F1} -> {ctx.Damage:F1} to {(comp ? comp.name : "attacker")}");
        // ВАЖНО: входящий урон игроку НЕ меняем — пассивка только отражает.
    }
}
