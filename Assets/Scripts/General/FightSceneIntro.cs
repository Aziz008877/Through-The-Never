using System;
using UnityEngine;

public class FightSceneIntro : MonoBehaviour
{
    [SerializeField] private FadeInOutEffect _fadeInOutEffect;
    private void Start()
    {
        _fadeInOutEffect.Fade(0);
    }
}
