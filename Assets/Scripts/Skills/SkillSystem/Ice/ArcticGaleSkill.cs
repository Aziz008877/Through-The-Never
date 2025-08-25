using System.Collections;
using UnityEngine;

public sealed class ArcticGalePassive : PassiveSkillBehaviour
{
    [Header("Buffs After Defense Ends")]
    [SerializeField, Range(0f, 1f)] private float _bonusMoveSpeedPercent = 0.30f;
    [SerializeField, Range(0f, 1f)] private float _damageReduction       = 0.30f;
    [SerializeField] private float _buffDuration = 3f;

    private ActiveSkillBehaviour _defense;
    private Coroutine _pending;
    private Coroutine _msBuffRoutine;
    private float _msBuffUntil;
    private float _drUntil;

    public override void EnablePassive()
    {
        Debug.Log("[ArcticGale] EnablePassive");
        HookDefense(Context.SkillManager.GetActive(SkillSlot.Defense));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;

        if (Context.Hp is PlayerHP php)
        {
            Debug.Log("[ArcticGale] Subscribed to OnIncomingDamage");
            php.OnIncomingDamage += OnIncomingDamage;
        }
    }

    public override void DisablePassive()
    {
        Debug.Log("[ArcticGale] DisablePassive");
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        UnhookDefense();

        if (_msBuffRoutine != null) { StopCoroutine(_msBuffRoutine); _msBuffRoutine = null; }
        PopMsBuff();

        if (Context.Hp is PlayerHP php)
        {
            Debug.Log("[ArcticGale] Unsubscribed from OnIncomingDamage");
            php.OnIncomingDamage -= OnIncomingDamage;
        }
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        Debug.Log($"[ArcticGale] OnActiveRegistered: {slot}");
        if (slot == SkillSlot.Defense) HookDefense(beh);
    }

    private void HookDefense(ActiveSkillBehaviour defense)
    {
        UnhookDefense();
        if (defense == null) return;
        _defense = defense;
        _defense.OnSkillActivated += OnDefenseActivated;
        Debug.Log("[ArcticGale] Hooked Defense skill");
    }

    private void UnhookDefense()
    {
        if (_defense != null)
        {
            _defense.OnSkillActivated -= OnDefenseActivated;
            Debug.Log("[ArcticGale] Unhooked Defense skill");
            _defense = null;
        }
        if (_pending != null) { StopCoroutine(_pending); _pending = null; }
    }

    private void OnDefenseActivated(float actualDuration)
    {
        Debug.Log($"[ArcticGale] Defense Activated, duration={actualDuration:F2}");
        if (_pending != null) StopCoroutine(_pending);
        _pending = StartCoroutine(WaitAndBuff(actualDuration));
    }

    private IEnumerator WaitAndBuff(float wait)
    {
        Debug.Log($"[ArcticGale] Waiting {wait:F2} sec before buff");
        if (wait > 0f) yield return new WaitForSeconds(wait);

        Debug.Log("[ArcticGale] Applying buffs after defense");
        ApplyMoveSpeedBuff(_bonusMoveSpeedPercent, _buffDuration);

        _drUntil = Time.time + _buffDuration;
        Debug.Log($"[ArcticGale] Damage Reduction active until {_drUntil:F2}");

        _pending = null;
    }

    private void ApplyMoveSpeedBuff(float percent, float duration)
    {
        if (_msBuffRoutine != null) StopCoroutine(_msBuffRoutine);
        _msBuffUntil = Time.time + Mathf.Max(0f, duration);
        _msBuffRoutine = StartCoroutine(MoveSpeedTick(percent));
    }

    private IEnumerator MoveSpeedTick(float percent)
    {
        float mul = 1f + Mathf.Max(0f, percent);
        var move = PlayerCtx?.Move as IExternalSpeedMul;

        Debug.Log($"[ArcticGale] MS buff started: +{percent:P0}, mul={mul}");

        move?.PushExternalSpeedMul(this, mul);

        while (Time.time < _msBuffUntil)
            yield return null;

        move?.PopExternalSpeedMul(this);
        Debug.Log("[ArcticGale] MS buff ended");

        _msBuffRoutine = null;
    }

    private void PopMsBuff()
    {
        var move = PlayerCtx?.Move as IExternalSpeedMul;
        move?.PopExternalSpeedMul(this);
        Debug.Log("[ArcticGale] Forced MS buff pop");
    }

    private void OnIncomingDamage(ref float dmg, IDamageable source)
    {
        if (Time.time <= _drUntil)
        {
            float newDmg = dmg * (1f - _damageReduction);
            Debug.Log($"[ArcticGale] Damage reduced {dmg:F1} -> {newDmg:F1}");
            dmg = newDmg;
        }
    }
}
