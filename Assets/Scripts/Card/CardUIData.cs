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
    [SerializeField] private RectTransform _fadeFrame;
    private Image _mainCardImage;

    private void Start()
    {
        _mainCardImage = GetComponent<Image>();
    }

    public void ReceiveCardData(SkillUIData skillUIData)
    {
        _skillIconImage.sprite = skillUIData.SkillImage;
        _skillNameText.text = skillUIData.SkillName;
        _skillDescriptionText.text = skillUIData.SkillDescription;
    }

    public void FadeOut()
    {
        _fadeFrame.gameObject.SetActive(true);
        _mainCardImage.DOFillAmount(0, 1);
        _fadeFrame.DOAnchorPosY(360, 1)
            .OnComplete(delegate
            {
                _fadeFrame.gameObject.SetActive(false);
            });
    }
}
