using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillPageUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Button _pickButton;

    private SkillDefinition _definition;
    private SkillChest _owner;

    public void Init(SkillDefinition definition, SkillChest owner)
    {
        _definition = definition;
        _owner = owner;

        _icon.sprite = definition.Icon;
        _name.text = definition.DisplayName;
        _description.text = definition.Description;
        _pickButton.onClick.AddListener(Pick);
    }

    private void Pick()
    {
        _owner.OnSkillPicked(_definition);
    }
}