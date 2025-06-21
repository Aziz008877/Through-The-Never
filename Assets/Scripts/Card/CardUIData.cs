using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIData : MonoBehaviour
{
    [SerializeField] private Image _skillIconImage;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    
    public void ReceiveCardData(SkillData skillData)
    {
        _skillIconImage.sprite = skillData.SkillImage;
        _skillNameText.text = skillData.SkillName;
        _skillDescriptionText.text = skillData.SkillDescription;
    }
}
