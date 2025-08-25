using System.Collections;
using UnityEngine;
public sealed class IfritRagePassive : PassiveSkillBehaviour, ISkillModifier, IDamageContextModifier
{
    [Header("Rage settings")]
    [SerializeField] private float _buffDuration = 5f;
    [SerializeField] private float _outgoingBonusPct = .30f;
    [SerializeField] private float _incomingBonusPct = .30f;
    private Coroutine _buffRoutine;
    private IDefenceDurationSkill _defSkill;
    public override void EnablePassive()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> enabled");
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(Context.SkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> disabled");
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        StopBuff();
        Detach();
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (!beh)
        {
            Debug.Log("<color=orange>[Ifrit’s Rage]</color> no DEF skill");
            return;
        }

        if (!beh.TryGetComponent(out IDefenceDurationSkill def))
        {
            Debug.Log("<color=orange>[Ifrit’s Rage]</color> DEF has no IDefenceDurationSkill");
            return;
        }

        _defSkill = def;
        _defSkill.OnDefenceStarted  += HandleDefenceStarted;
        _defSkill.OnDefenceFinished += HandleDefenceFinished;

        HandleDefenceStarted();
    }

    private void Detach()
    {
        if (_defSkill == null) return;
        _defSkill.OnDefenceStarted  -= HandleDefenceStarted;
        _defSkill.OnDefenceFinished -= HandleDefenceFinished;
        _defSkill = null;
    }
    
    private void HandleDefenceStarted()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> DEF started → rage armed");
        StartOrRestartBuff();
    }

    private void HandleDefenceFinished()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> DEF finished → rage timer reset");
        StartOrRestartBuff();
    }

    private void StartOrRestartBuff()
    {
        StopBuff();
        _buffRoutine = StartCoroutine(RageBuffRoutine());
    }

    private IEnumerator RageBuffRoutine()
    {
        Context.SkillModifierHub.Register(this);
        Context.RegisterContextModifier(this);
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> RAGE ON");

        yield return new WaitForSeconds(_buffDuration);

        Debug.Log("<color=orange>[Ifrit’s Rage]</color> rage expired");
        StopBuff();
    }

    private void StopBuff()
    {
        if (_buffRoutine != null)
        {
            StopCoroutine(_buffRoutine);
            _buffRoutine = null;
        }
        Context.SkillModifierHub.Unregister(this);
        Context.UnregisterContextModifier(this);
    }
    
    public float Evaluate(SkillKey key, float value)
    {
        if (_buffRoutine != null &&
            key.Stat == SkillStat.Damage &&
            key.Slot != SkillSlot.Passive)
        {
            float boosted = value * (1f + _outgoingBonusPct);
            Debug.Log($"<color=orange>[Ifrit’s Rage]</color> OUT +{_outgoingBonusPct:P0} → {boosted:F1}");
            return boosted;
        }
        return value;
    }

    public void Apply(ref DamageContext ctx)
    {
        if (_buffRoutine == null) return;

        float extra = ctx.Damage * _incomingBonusPct;
        ctx.Damage += extra;
        Debug.Log($"<color=orange>[Ifrit’s Rage]</color> IN +{_incomingBonusPct:P0} → {ctx.Damage:F1}");
    }

    private void OnDisable() => DisablePassive();
}

public interface IDefenceDurationSkill
{
    event System.Action OnDefenceStarted;
    event System.Action OnDefenceFinished;
}
