using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIHandler : MonoBehaviour
{
    [Header("UI Icons")]
    [SerializeField] private Image _basicIcon;
    [SerializeField] private Image _defenseIcon;
    [SerializeField] private Image _specialIcon;
    [SerializeField] private Image _dashIcon;

    [Header("Data")] [SerializeField] private SkillSelectionSaver _saver;

    private void OnEnable()
    {
        SyncAllFromSaver();
    }

    public void SetSaver(SkillSelectionSaver saver)
    {
        _saver = saver;
        SyncAllFromSaver();
    }

    public void SyncAllFromSaver()
    {
        if (_saver == null) return;

        var chosen = _saver.Chosen;
        if (chosen == null) return;
        
        for (int i = 0; i < chosen.Count; i++)
        {
            var def = chosen[i];
            if (!def) continue;
            ReceiveNewSkill(def);
        }
    }

    public void ReceiveNewSkill(SkillDefinition skillDefinition)
    {
        if (!skillDefinition) return;

        switch (skillDefinition.Slot)
        {
            case SkillSlot.Basic:
                if (_basicIcon) _basicIcon.sprite = skillDefinition.Icon;
                break;

            case SkillSlot.Defense:
                if (_defenseIcon) _defenseIcon.sprite = skillDefinition.Icon;
                break;

            case SkillSlot.Special:
                if (_specialIcon) _specialIcon.sprite = skillDefinition.Icon;
                break;

            case SkillSlot.Dash:
                if (_dashIcon) _dashIcon.sprite = skillDefinition.Icon;
                break;
        }
    }
}