using System.Collections;
using UnityEngine;

public sealed class SolarBlessingPassive : PassiveSkillBehaviour
{
    [Header("Regen")]
    [SerializeField] private float _healPerTick = 3f;
    [SerializeField] private float _tickRate    = 1f;
    private ActiveSkillBehaviour _defSkill;
    private Coroutine _regenRoutine;
    public override void EnablePassive()
    {
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        TryAttach(Context.SkillManager.GetActive(SkillSlot.Defense));
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        StopRegen();
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Defense) TryAttach(beh);
    }

    private void TryAttach(ActiveSkillBehaviour beh)
    {
        Detach();

        _defSkill = beh;
        if (_defSkill == null) return;

        if (!_defSkill.IsReady)
            StartRegen(_defSkill.RemainingCooldown);

        _defSkill.OnSkillActivated += StartRegen;
    }

    private void Detach()
    {
        if (_defSkill == null) return;
        _defSkill.OnSkillActivated -= StartRegen;
        _defSkill = null;
    }

    private void StartRegen(float cdSeconds)
    {
        Debug.Log(cdSeconds);
        StopRegen();
        _regenRoutine = StartCoroutine(RegenRoutine(cdSeconds));
    }

    private IEnumerator RegenRoutine(float total)
    {
        var wait = new WaitForSeconds(_tickRate);
        float elapsed = 0f;

        while (elapsed < total)
        {
            Context.Hp.ReceiveHP(_healPerTick);
            elapsed += _tickRate;
            yield return wait;
        }
        _regenRoutine = null;
    }

    private void StopRegen()
    {
        if (_regenRoutine == null) return;
        StopCoroutine(_regenRoutine);
        _regenRoutine = null;
    }
}
