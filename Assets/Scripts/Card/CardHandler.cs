using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class CardHandler : MonoBehaviour
{
    [SerializeField] private CardUIData[] _allCards;
    [SerializeField] private SkillUIData[] _skillDatas;
    [SerializeField] private Chest _chest;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private DotweenSettings _dotweenSettings;
    [Inject] private PlayerSkillHandler _playerSkillHandler;
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
        ShowCards();
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

    public void ApplySkill(int skillID)
    {
        _cameraShake.Shake();
        /*_playerSkillHandler.ApplySelectedSkill(skillID);
        
        for (int i = 0; i < _allCards.Length; i++)
        {
            if (i == skillID)
            {
                continue;
            }
            _allCards[i].FadeOut();
        }*/
    }
}