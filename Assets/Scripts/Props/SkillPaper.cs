using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SkillPaper : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _skillName;
    [SerializeField] private TMP_Text _skillDescription;

    public SkillDefinition Definition { get; private set; }

    public event Action<SkillPaper> Clicked;
    [SerializeField] private UnityEvent<SkillPaper> _onSelected;

    public void Init(SkillDefinition def)
    {
        Definition = def;

        if (def == null)
        {
            if (_skillName)        _skillName.text = "";
            if (_skillDescription) _skillDescription.text = "";
            gameObject.SetActive(false);
            return;
        }

        if (_skillName)        _skillName.text = def.DisplayName;
        if (_skillDescription) _skillDescription.text = def.Description;

        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Select()
    {
        if (Definition == null) return;
        Clicked?.Invoke(this);
        _onSelected?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) => Select();
}