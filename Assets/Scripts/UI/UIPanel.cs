using System;
using DG.Tweening;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField] private Vector2 _activePosition, _deactivePosition;
    [SerializeField] private DotweenSettings _dotweenSettings;
    public event Action OnMoveFinished;
    private RectTransform _rect;
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void ActivatePanel()
    {
        Move(_activePosition);
    }

    public void ActivateWithCallback(Action callback)
    {
        MoveWithCallback(_activePosition, callback);
    }
    
    public void DeactivatePanel()
    {
        Move(_deactivePosition);
    }
    
    public void DeactivateWithCallback(Action callback)
    {
        MoveWithCallback(_deactivePosition, callback);
    }

    private void Move(Vector2 position)
    {
        if (!_isMoving)
        {
            _isMoving = true;

            _rect
                .DOAnchorPos(position, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType)
                .OnComplete(delegate
                {
                    _isMoving = false;
                    OnMoveFinished?.Invoke();
                });
        }
    }
    
    private void MoveWithCallback(Vector2 position, Action callback)
    {
        if (!_isMoving)
        {
            _isMoving = true;

            _rect
                .DOAnchorPos(position, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType)
                .OnComplete(delegate
                {
                    _isMoving = false;
                    OnMoveFinished?.Invoke();
                    callback?.Invoke();
                });
        }
    }

}
