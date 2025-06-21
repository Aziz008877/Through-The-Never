using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PapirusIntro : MonoBehaviour
{
    [SerializeField] private Button _papirusCloseButton;
    [SerializeField] private CanvasGroup _canvasGroup, _buttonCanvasGroup;
    [SerializeField] private GameObject _introCanvas;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private void Start()
    {
        _canvasGroup
            .DOFade(1, _dotweenSettings.Duration)
            .SetEase(_dotweenSettings.AnimationType)
            .OnComplete(delegate
            {
                _buttonCanvasGroup.DOFade(1, _dotweenSettings.Duration);
            });
        
        _papirusCloseButton.onClick.AddListener(CloseIntro);
    }

    private void CloseIntro()
    {
        _canvasGroup
            .DOFade(0, 1)
            .SetEase(_dotweenSettings.AnimationType)
            .OnComplete(delegate
            {
                _introCanvas.SetActive(false);
            });
    }
}
