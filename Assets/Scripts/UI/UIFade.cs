using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private DotweenSettings _dotweenSettings;
    public void Fade(float fadeValue)
    {
        _fadeImage
            .DOFade(fadeValue, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType);
    }

    public void RaycastState(bool state)
    {
        _fadeImage.raycastTarget = state;
    }
}
