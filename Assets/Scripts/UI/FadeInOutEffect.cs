using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class FadeInOutEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private DotweenSettings _dotweenSettings;

    public void FadeInOut(float time)
    {
        StartCoroutine(FadeInFadeOut(time));
    }

    public void Fade(float fadeValue)
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(fadeValue, _dotweenSettings.Duration);
    }
    
    private IEnumerator FadeInFadeOut(float time)
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(1, _dotweenSettings.Duration);

        yield return new WaitForSeconds(time);

        _fadeImage.DOFade(0, _dotweenSettings.Duration)
            .OnComplete(delegate
            {
                _fadeImage.gameObject.SetActive(false);
            });
    }
}
