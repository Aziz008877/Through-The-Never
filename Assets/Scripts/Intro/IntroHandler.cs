using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class IntroHandler : MonoBehaviour
{
    [SerializeField] private UIPanel _panel;
    [SerializeField] private UIPanel[] _intros;
    [SerializeField] private Image _uiFade;
    [SerializeField] private Camera _camera;
    [SerializeField] private DotweenSettings _sunDotweenSettings;
    [SerializeField] private DotweenSettings _imagesDotweenSettings;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        
        _camera.transform
            .DORotate(new Vector3(60, 0, 0), 2)
            .SetEase(_sunDotweenSettings.AnimationType);
        
        _uiFade
            .DOFade(1, 2)
            .SetEase(_sunDotweenSettings.AnimationType);
        
        yield return new WaitForSeconds(3);
        
        _uiFade
            .DOFade(0, _sunDotweenSettings.Duration)
            .SetEase(Ease.Linear);

        StartCoroutine(MovePictures());
    }

    private IEnumerator MovePictures()
    {
        _intros[0].gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        _intros[0].ActivatePanel();
        yield return new WaitForSeconds(4);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(3);
        _uiFade
            .DOFade(0, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        _intros[1].gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        _intros[1].ActivatePanel();
        yield return new WaitForSeconds(4);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(3);
        _uiFade
            .DOFade(0, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        _intros[2].gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        _intros[2].ActivatePanel();
        yield return new WaitForSeconds(4);
        _uiFade
            .DOFade(1, _imagesDotweenSettings.Duration)
            .SetEase(_imagesDotweenSettings.AnimationType);
        yield return new WaitForSeconds(3);
    }
}
