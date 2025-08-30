using System;
using System.Collections.Generic;
using UnityEngine;

public class AquaPrimePassive : PassiveSkillBehaviour
{
    [Header("Cooldown haste while Defense is active")]
    [SerializeField, Range(0f, 1f)]
    private float _cooldownHastePercent = 0.35f;

    private ActiveSkillBehaviour _defense;
    private readonly List<ActiveSkillBehaviour> _actives = new();
    private float _hasteUntil = -1f;

    public override void EnablePassive()
    {
        RefreshActives();
        HookDefense(Context.SkillManager.GetActive(SkillSlot.Defense));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        UnhookDefense();
        _actives.Clear();
        _hasteUntil = -1f;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (beh != null && beh.Context == Context && !_actives.Contains(beh))
            _actives.Add(beh);

        if (slot == SkillSlot.Defense) HookDefense(beh);
    }

    private void RefreshActives()
    {
        _actives.Clear();
        foreach (SkillSlot s in Enum.GetValues(typeof(SkillSlot)))
        {
            var a = Context.SkillManager.GetActive(s);
            if (a != null && a.Context == Context && !_actives.Contains(a))
                _actives.Add(a);
        }
    }

    private void HookDefense(ActiveSkillBehaviour defense)
    {
        UnhookDefense();
        if (defense == null) return;

        _defense = defense;
        _defense.OnSkillActivated += OnDefenseActivated;
    }

    private void UnhookDefense()
    {
        if (_defense != null)
        {
            _defense.OnSkillActivated -= OnDefenseActivated;
            _defense = null;
        }
    }

    private void OnDefenseActivated(float duration)
    {
        _hasteUntil = Time.time + Mathf.Max(0f, duration);
    }
    
    protected void Update()
    {
        if (Time.time >= _hasteUntil) return;

        float extraDt = Time.deltaTime * Mathf.Clamp01(_cooldownHastePercent);
        if (extraDt <= 0f) return;

        for (int i = 0; i < _actives.Count; i++)
        {
            var s = _actives[i];
            if (s == null) continue;

            float rem = s.RemainingCooldown;
            if (rem > 0f)
                s.SetCooldown(rem - extraDt);
        }
    }
}
