using System.Collections;
using UnityEngine;
public sealed class IfritRagePassive :
    PassiveSkillBehaviour, ISkillModifier, IDamageModifier
{
    [Header("Rage settings")]
    [SerializeField] private float _buffDuration = 5f;
    [SerializeField] private float _outgoingBonusPct = .30f;
    [SerializeField] private float _incomingBonusPct = .30f;
    private Coroutine _buffRoutine;
    private IDefenceDurationSkill _attachedDef;
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        StopBuff();
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour defenceSkill)
    {
        Detach();

        if (defenceSkill != null && defenceSkill.TryGetComponent<IDefenceDurationSkill>(out var def))
        {
            def.OnDefenceFinished += HandleDefenceFinished;
            _attachedDef = def;
        }
    }

    private void Detach()
    {
        if (_attachedDef != null)
        {
            _attachedDef.OnDefenceFinished -= HandleDefenceFinished;
            _attachedDef = null;
        }
    }

    private void HandleDefenceFinished()
    {
        StopBuff();
        _buffRoutine = StartCoroutine(RageBuffRoutine());
    }

    private IEnumerator RageBuffRoutine()
    {
        PlayerContext.SkillModifierHub.Register(this);
        PlayerContext.RegisterModifier(this);
        float timer = 0f;

        while (timer < _buffDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StopBuff();
    }

    private void StopBuff()
    {
        if (_buffRoutine != null)
        {
            StopCoroutine(_buffRoutine);
            _buffRoutine = null;
        }
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.UnregisterModifier(this);
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_buffRoutine != null && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
            return currentValue * (1f + _outgoingBonusPct);
        return currentValue;
    }

    public void Apply(ref float dmg, ref SkillDamageType type)
    {
        if (_buffRoutine != null)
            dmg *= (1f + _incomingBonusPct);
    }
}
public interface IDefenceDurationSkill
{
    event System.Action OnDefenceFinished;
}