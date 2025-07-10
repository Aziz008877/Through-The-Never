using UnityEngine;
public class JetfirePassive : PassiveSkillBehaviour, ISkillModifier
{
    [Header("Jet-fire Settings")]
    [SerializeField] private float _moveSpeedMultiplier = 1.5f;
    [SerializeField] private float _cooldownMultiplier  = 0.75f;
    [SerializeField] private float _buffDuration = 10f;
    private bool  _buffActive;
    private float _timeLeft;
    public override void EnablePassive()
    {
        PlayerContext.SkillModifierHub.Register(this);
        //добавить ивент о начале боя и вызывать OnCombatStart
    }

    public override void DisablePassive()
    {
        PlayerContext.SkillModifierHub.Unregister(this);

        StopBuff();
    }
    
    private void Update()
    {
        if (!_buffActive) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
            StopBuff();
    }

    private void OnCombatStart()
    {
        _buffActive = true;
        _timeLeft = _buffDuration;

        PlayerContext.PlayerMove.SetSpeedMultiplier(_moveSpeedMultiplier);
    }

    private void StopBuff()
    {
        if (!_buffActive) return;

        _buffActive = false;
        PlayerContext.PlayerMove.SetSpeedMultiplier(1f);
    }
    
    public float Evaluate(SkillKey key, float baseValue)
    {
        if (!_buffActive) return baseValue;
        
        if (key.Stat == SkillStat.Cooldown && key.Slot != SkillSlot.Passive)
            return baseValue * _cooldownMultiplier;

        return baseValue;
    }
}
