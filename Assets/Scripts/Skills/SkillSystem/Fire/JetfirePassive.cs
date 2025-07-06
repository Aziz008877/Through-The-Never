using UnityEngine;

public class JetfirePassive : PassiveSkillBehaviour, ISkillModifier
{
    [SerializeField] private float _speedMultiplier = 1.5f;

    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);

        AttachToExistingDash();
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnActiveRegistered;
    }

    void AttachToExistingDash()
    {
        if (PlayerContext.PlayerSkillManager.Actives.TryGetValue(
                SkillSlot.Dash, out var behaviour) &&
            behaviour.TryGetComponent<PlayerDashSkill>(out var dash))
        {
            dash.SetSpeedMultiplier(_speedMultiplier);
        }
    }

    void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour behaviour)
    {
        if (slot == SkillSlot.Dash &&
            behaviour.TryGetComponent<PlayerDashSkill>(out var dash))
        {
            dash.SetSpeedMultiplier(_speedMultiplier);
            Debug.Log("[Jetfire] multiplier applied via event");
        }
    }

    // ——————————————— ISkillModifier ———————————————
    public float Evaluate(SkillKey key, float currentValue)
    {
        if (key.Slot == SkillSlot.Dash && key.Stat == SkillStat.Cooldown)
            return currentValue / _speedMultiplier;      // быстрее → меньше КД
        return currentValue;
    }
}