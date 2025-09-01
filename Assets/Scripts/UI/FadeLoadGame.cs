using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeLoadGame : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Image _uiFade;
    [SerializeField] private DotweenSettings _dotweenSettings;
    private void Start()
    {
        _startGameButton.onClick.AddListener(delegate
        {
            _uiFade
                .DOFade(1, _dotweenSettings.Duration)
                .SetEase(_dotweenSettings.AnimationType)
                .SetDelay(1)
                .OnComplete(delegate
                {
                    SceneManager.LoadScene("Intro");
                });
        });
    }
}
