using System.Collections;
using UnityEngine;

public sealed class IfritRagePassive : PassiveSkillBehaviour,
                                       ISkillModifier, IDamageModifier
{
    /*──────── настройки ────────*/
    [Header("Rage settings")]
    [SerializeField] private float _buffDuration      = 5f;
    [SerializeField] private float _outgoingBonusPct  = .30f;   // +30 % к нашему урону
    [SerializeField] private float _incomingBonusPct  = .30f;   // +30 % входящего

    private Coroutine              _buffRoutine;
    private IDefenceDurationSkill  _attachedDef;

    /*──────── включение / выключение пассивки ────────*/
    public override void EnablePassive()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> enabled");
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        StopBuff();
        Detach();
    }

    /*──────── привязка к текущему DEF-скиллу ────────*/
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour defenceSkill)
    {
        Detach();

        if (defenceSkill != null && defenceSkill.TryGetComponent<IDefenceDurationSkill>(out var def))
        {
            Debug.Log("<color=orange>[Ifrit’s Rage]</color> attached to DEF skill");
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

    /*──────── DEF-скилл закончился ────────*/
    private void HandleDefenceFinished()
    {
        Debug.Log("<color=orange>[Ifrit’s Rage]</color> DEF finished → start rage");
        StopBuff();                                   // перестраховка
        _buffRoutine = StartCoroutine(RageBuffRoutine());
    }

    private IEnumerator RageBuffRoutine()
    {
        PlayerContext.SkillModifierHub.Register(this);      // + outgoing
        PlayerContext.RegisterModifier(this);               // + incoming

        float timer = 0f;
        while (timer < _buffDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

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
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.UnregisterModifier(this);
    }

    /*──────── ISkillModifier – повышаем свой урон ────────*/
    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_buffRoutine != null &&
            key.Stat == SkillStat.Damage &&
            key.Slot != SkillSlot.Passive)
        {
            float boosted = currentValue * (1f + _outgoingBonusPct);
            Debug.Log($"<color=orange>[Ifrit’s Rage]</color> OUT +{_outgoingBonusPct:P0} → {boosted:F1}");
            return boosted;
        }
        return currentValue;
    }

    /*──────── IDamageModifier – усиливаем входящий урон ────────*/
    public void Apply(ref float dmg, ref SkillDamageType type)
    {
        if (_buffRoutine == null) return;

        float extra = dmg * _incomingBonusPct;
        dmg += extra;
        Debug.Log($"<color=orange>[Ifrit’s Rage]</color> IN +{_incomingBonusPct:P0} → {dmg:F1}");
    }

    private void OnDisable()
    {
        if (_buffRoutine != null || _attachedDef != null)
            DisablePassive();
    }
}

public interface IDefenceDurationSkill
{
    event System.Action OnDefenceStarted;
    event System.Action OnDefenceFinished;
}
