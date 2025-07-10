using System.Collections;
using UnityEngine;
public sealed class SolarBlessingPassive : PassiveSkillBehaviour
{
    [Header("Regen")]
    [SerializeField] private float _healPerTick = 3f;
    [SerializeField] private float _tickRate = 1f;
    private ActiveSkillBehaviour _defenseSkill;
    private Coroutine _regenRoutine;
    public override void EnablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(PlayerContext.PlayerSkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
        StopRegen();
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        if (slot == SkillSlot.Defense) TryAttach(behaviour);
    }

    private void TryAttach(ActiveSkillBehaviour behaviour)
    {
        if (_defenseSkill != null)
        {
            _defenseSkill.OnCooldownStarted -= StartRegen;
            StopRegen();
        }

        _defenseSkill = behaviour;
        if (_defenseSkill == null) return;

        if (!_defenseSkill.IsReady) StartRegen(_defenseSkill.Definition.Cooldown);

        _defenseSkill.OnCooldownStarted += StartRegen;
    }

    private void StartRegen(float totalCd)
    {
        StopRegen();
        _regenRoutine = StartCoroutine(RegenRoutine(totalCd));
    }

    private IEnumerator RegenRoutine(float totalCd)
    {
        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);

        while (elapsed < totalCd)
        {
            PlayerContext.PlayerHp.ReceiveHP(_healPerTick);
            elapsed += _tickRate;
            yield return wait;
        }

        _regenRoutine = null;
    }

    private void StopRegen()
    {
        if (_regenRoutine != null)
        {
            StopCoroutine(_regenRoutine);
            _regenRoutine = null;
        }
    }
}
