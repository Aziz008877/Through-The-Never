using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public sealed class VignetteEffect : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private float _fadeTime = .15f;
    [SerializeField] private float _holdTime = .2f;
    [SerializeField] private float _intensity = .45f;
    private Vignette _vignette;

    private void Awake()
    {
        _volume.profile.TryGet(out _vignette);
        PlayerSkillManager.OnSkillPerformed += OnSkillCast;
    }
    
    private void OnSkillCast(SkillSlot skill)
    {
        if (skill != SkillSlot.Special) return;
        StopAllCoroutines();
        StartCoroutine(VignetteRoutine());
    }
    
    private IEnumerator VignetteRoutine()
    {
        yield return Fade(0, _intensity, _fadeTime);
        yield return new WaitForSeconds(_holdTime);
        yield return Fade(_intensity, 0, _fadeTime);
    }
    
    private IEnumerator Fade(float from, float to, float time)
    {
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            _vignette.intensity.value = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        _vignette.intensity.value = to;
    }

    private void OnDestroy()
    {
        PlayerSkillManager.OnSkillPerformed -= OnSkillCast;
    }
}