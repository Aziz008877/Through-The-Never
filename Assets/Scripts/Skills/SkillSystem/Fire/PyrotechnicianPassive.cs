using System.Collections;
using UnityEngine;
public sealed class PyrotechnicianPassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _damageBonusPercent = 0.30f;
    private ActiveSkillBehaviour _defenseSkill;
    private Coroutine _bonusRoutine;
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
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_bonusActive &&
            key.Stat == SkillStat.Damage &&
            key.Slot != SkillSlot.Passive)
        {
            currentValue *= 1f + Mathf.Max(0f, _damageBonusPercent);
        }
        return currentValue;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        if (slot == SkillSlot.Defense) TryAttach(behaviour);
    }

    private void TryAttach(ActiveSkillBehaviour behaviour)
    {
        if (_defenseSkill != null)
        {
            _defenseSkill.OnCooldownStarted -= ActivateBonus;
            DeactivateBonus();
        }

        _defenseSkill = behaviour;
        if (_defenseSkill == null) return;
        
        if (!_defenseSkill.IsReady) ActivateBonus(_defenseSkill.Definition.Cooldown);

        _defenseSkill.OnCooldownStarted += ActivateBonus;
    }

    private void ActivateBonus(float totalCd)
    {
        if (_bonusActive) DeactivateBonus();

        _bonusActive = true;
        PlayerContext.SkillModifierHub.Register(this);
        _bonusRoutine = StartCoroutine(BonusTimer(totalCd));
    }

    private IEnumerator BonusTimer(float totalCd)
    {
        yield return new WaitForSeconds(totalCd);
        DeactivateBonus();
    }

    private void DeactivateBonus()
    {
        if (!_bonusActive) return;

        _bonusActive = false;
        PlayerContext.SkillModifierHub.Unregister(this);

        if (_bonusRoutine != null)
        {
            StopCoroutine(_bonusRoutine);
            _bonusRoutine = null;
        }
    }
}
