using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardFlip : MonoBehaviour
{
    [SerializeField] private GameObject[] _revealObjects;
    [SerializeField] private Button _cardPressButton;
    [SerializeField] private Image _renderer;
    [SerializeField] private Sprite _frontSprite;
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private float _flipDuration = 0.5f;
    [SerializeField] private int _skillID;
    [SerializeField] private CardHandler _cardHandler;
    private bool _isFlipped = true;
    private bool _isAnimating = false;
    private bool _hasFlippedOnce = false;

    private void Start()
    {
        _cardPressButton.onClick.AddListener(OnCardPressed);
    }

    private void OnCardPressed()
    {
        if (_isAnimating) return;

        if (!_hasFlippedOnce)
        {
            Flip();
            _hasFlippedOnce = true;
        }
        else
        {
            Choose();
        }
    }

    private void Flip()
    {
        _isAnimating = true;

        transform.DOScaleX(0f, _flipDuration / 2f).OnComplete(() =>
        {
            _isFlipped = !_isFlipped;
            _renderer.sprite = _isFlipped ? _backSprite : _frontSprite;

            foreach (var reveal in _revealObjects)
            {
                reveal.SetActive(true);
            }
            
            transform.DOScaleX(1f, _flipDuration / 2f).OnComplete(() =>
            {
                _isAnimating = false;
            });
        });
    }

    private void Choose()
    {
        _cardHandler.ApplySkill(_skillID);
    }
}