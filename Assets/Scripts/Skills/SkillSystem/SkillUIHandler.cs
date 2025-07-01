using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class SkillUIHandler : MonoBehaviour
{
    [Header("UI Icons")]
    [SerializeField] private Image _basicIcon;
    [SerializeField] private Image _defenseIcon;
    [SerializeField] private Image _specialIcon;
    [SerializeField] private Image _dashIcon;

    [Header("Cooldown Masks (type = Filled)")]
    [SerializeField] private Image _basicCooldown;
    [SerializeField] private Image _defenseCooldown;
    [SerializeField] private Image _specialCooldown;
    [SerializeField] private Image _dashCooldown;

    private readonly Dictionary<SkillSlot, Image> _iconBySlot      = new();
    private readonly Dictionary<SkillSlot, Image> _cooldownBySlot  = new();

    private void Awake()
    {
        _iconBySlot[SkillSlot.Basic]    = _basicIcon;
        _iconBySlot[SkillSlot.Defense]  = _defenseIcon;
        _iconBySlot[SkillSlot.Special]  = _specialIcon;
        _iconBySlot[SkillSlot.Dash]     = _dashIcon;

        _cooldownBySlot[SkillSlot.Basic]   = _basicCooldown;
        _cooldownBySlot[SkillSlot.Defense] = _defenseCooldown;
        _cooldownBySlot[SkillSlot.Special] = _specialCooldown;
        _cooldownBySlot[SkillSlot.Dash]    = _dashCooldown;
    }
    
    public void Populate(List<SkillDefinition> definitions, Dictionary<SkillSlot, ActiveSkillBehaviour> actives)
    {
        foreach (SkillDefinition definition in definitions)
        {
            if (_iconBySlot.TryGetValue(definition.Slot, out var icon))
                icon.sprite = definition.Icon;

            if (definition.Kind != SkillKind.Active) continue;
            if (!actives.TryGetValue(definition.Slot, out var behaviour)) continue;
            if (!_cooldownBySlot.TryGetValue(definition.Slot, out var mask) || mask == null) continue;
            
            mask.fillAmount = behaviour.IsReady ? 0f : 1f;

            behaviour.OnCooldownStarted += seconds =>
            {
                mask.DOKill();
                mask.fillAmount = 1f;
                mask.DOFillAmount(0f, seconds)
                    .SetEase(Ease.Linear);
            };
        }
    }
}
