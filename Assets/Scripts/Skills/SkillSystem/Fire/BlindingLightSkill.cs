using System.Collections;
using UnityEngine;

public class BlindingLightSkill : ActiveSkillBehaviour
{
    [Header("Blinding Light Settings")]
    [SerializeField] private float _radius = 7f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _baseDps = 10f;
    [SerializeField] private ParticleSystem _vfx;

    public override void TryCast()
    {
        if (!IsReady) return;
        StartCoroutine(BlindingRoutine());
        StartCooldown();
    }

    private IEnumerator BlindingRoutine()
    {
        if (_vfx) _vfx.Play();

        float timer = 0f;
        while (timer < _duration)
        {
            ApplyBlindingEffect();
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (_vfx) _vfx.Stop();
    }

    private void ApplyBlindingEffect()
    {
        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, _radius);
        foreach (var col in hits)
        {
            if (!col.TryGetComponent(out IBlindable enemy)) continue;

            float dist = Vector3.Distance(PlayerContext.transform.position, col.transform.position);
            float proximity = Mathf.Clamp01(1f - dist / _radius);

            enemy.ApplyBlind(_duration, 0.5f, 0.5f, _baseDps * proximity);
        }
    }
}