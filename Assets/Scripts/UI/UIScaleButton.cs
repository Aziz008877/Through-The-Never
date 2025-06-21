using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIScaleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _scaleFactor = 1.2f;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private Vector3 _originalScale;
    private Tween _scaleTween;
    private bool _canScale = true;
    private void Awake()
    {
        _originalScale = transform.localScale;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_canScale) return;
        
        _scaleTween?.Kill(); 
        _scaleTween = transform.DOScale(_originalScale * _scaleFactor, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_canScale) return;
        
        _scaleTween = transform.DOScale(_originalScale, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType);
    }

    public void ChangeCanScale(bool canScale)
    {
        _canScale = canScale;
        _scaleTween?.Kill(); 
        
        if (_canScale)
        {
            _scaleTween = transform.DOScale(_originalScale, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
        }
        else
        {
            _scaleTween = transform.DOScale(_originalScale * _scaleFactor, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType);
        }
    }
}
