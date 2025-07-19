using System.Collections;
using UnityEngine;
public sealed class PyrotechnicianPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _damageBonusPercent = 0.30f;

    private IDefenceDurationSkill _def;
    private bool _bonusActive;

    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        DeactivateBonus();
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    /*──────── ISkillModifier ────────*/
    public float Evaluate(SkillKey key, float value)
    {
        if (_bonusActive && key.Stat == SkillStat.Damage && key.Slot != SkillSlot.Passive)
            return value * (1f + _damageBonusPercent);
        return value;
    }

    /*──────── подписка на DEF ────────*/
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh && beh.TryGetComponent(out IDefenceDurationSkill def))
        {
            _def = def;
            _def.OnDefenceStarted  += ActivateBonus;
            _def.OnDefenceFinished += DeactivateBonus;
        }
    }

    private void Detach()
    {
        if (_def == null) return;
        _def.OnDefenceStarted  -= ActivateBonus;
        _def.OnDefenceFinished -= DeactivateBonus;
        _def = null;
    }

    /*──────── бонус ────────*/
    private void ActivateBonus()
    {
        if (_bonusActive) return;
        _bonusActive = true;
        PlayerContext.SkillModifierHub.Register(this);
        Debug.Log("<color=orange>[Pyrotechnician]</color> bonus ON");
    }

    private void DeactivateBonus()
    {
        if (!_bonusActive) return;
        _bonusActive = false;
        PlayerContext.SkillModifierHub.Unregister(this);
        Debug.Log("<color=orange>[Pyrotechnician]</color> bonus OFF");
    }
}
