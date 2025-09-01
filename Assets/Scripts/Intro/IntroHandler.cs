using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class IntroHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _intros;
    [SerializeField] private Image _uiFade;
    [SerializeField] private AudioSource _narratorPitch;
    [SerializeField] private DotweenSettings _sunDotweenSettings;
    [SerializeField] private DotweenSettings _imagesDotweenSettings;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);

        _uiFade
            .DOFade(1, 2)
            .SetEase(_sunDotweenSettings.AnimationType);
        
        yield return new WaitForSeconds(3);
        
        _uiFade
            .DOFade(0, _sunDotweenSettings.Duration)
            .SetEase(Ease.Linear);

        StartCoroutine(MovePictures());
        StartCoroutine(Fade());
    }

    private IEnumerator MovePictures()
    {
        _narratorPitch.Play();
        _intros[0].SetActive(true);
        yield return new WaitForEndOfFrame();
        _intros[0].GetComponent<UIPanel>().ActivatePanel();
        yield return new WaitForSeconds(10);
        _intros[1].SetActive(true);
        yield return new WaitForSeconds(5f);
        _intros[2].SetActive(true);
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(6);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(4);
        _uiFade
            .DOFade(0, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(4);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(1);
        _uiFade
            .DOFade(0, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(6);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType)
            .SetDelay(1)
            .OnComplete(delegate
            {
                SceneManager.LoadScene("Game");
            });
    }
}
