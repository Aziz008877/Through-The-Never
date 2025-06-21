using System;
using DG.Tweening;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    [SerializeField] private CardUIData[] _allCards;
    [SerializeField] private SkillData[] _skillDatas;
    [SerializeField] private Chest _chest;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private DotweenSettings _dotweenSettings;

    private bool _cardsShown = false;

    private void Awake()
    {
        _chest.OnChestOpened += OnChestInteraction;
    }

    private void OnDestroy()
    {
        _chest.OnChestOpened -= OnChestInteraction;
    }

    private void OnChestInteraction()
    {
        if (!_cardsShown)
        {
            ShowCards();
            _cardsShown = true;
        }
        else
        {
            ApplySkills();
        }
    }

    private void ShowCards()
    {
        _canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            for (int i = 0; i < _allCards.Length; i++)
            {
                _allCards[i].ReceiveCardData(_skillDatas[i]);

                float delay = i * 0.5f;

                _allCards[i].transform.DOScale(Vector3.one, _dotweenSettings.Duration)
                    .SetDelay(delay)
                    .SetEase(_dotweenSettings.AnimationType);
            }
        });
    }

    private void ApplySkills()
    {
        foreach (var skill in _skillDatas)
        {
            Debug.Log("Skill applied: " + skill.SkillName);
            // Вставь здесь настоящую логику применения навыков
        }

        _canvasGroup.DOFade(0, 0.5f);
    }
}