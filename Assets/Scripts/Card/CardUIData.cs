using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIData : MonoBehaviour
{
    [SerializeField] private Image _skillIconImage;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private CanvasGroup _canvasGroup;
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ReceiveCardData(SkillUIData skillUIData)
    {
        _skillIconImage.sprite = skillUIData.SkillImage;
        _skillNameText.text = skillUIData.SkillName;
        _skillDescriptionText.text = skillUIData.SkillDescription;
    }

    public void FadeOut(float value)
    {
        _canvasGroup.DOFade(value, _dotweenSettings.Duration)
            .OnComplete(delegate
            {
                transform.DOMoveY(3000, 0.5f);
            });
    }
}
