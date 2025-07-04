using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebreathSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _breathVfx;

    [Header("Shape")]
    [SerializeField] private float _coneAngle = 45f;
    [SerializeField] private float _coneRange = 8f;

    [Header("Timing & DPS")]
    [SerializeField] private float _tickDamage = 2f;
    [SerializeField] private float _tickRate = 0.25f;
    private readonly HashSet<IDamageable> _burning = new();

    public override void TryCast()
    {
        if (!IsReady) return;
        StartCoroutine(BreathRoutine());
        StartCooldown();
    }

    private IEnumerator BreathRoutine()
    {
        _breathVfx.Play();
        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);

        while (elapsed < Definition.Duration)
        {
            DealConeDamage();
            elapsed += _tickRate;
            yield return wait;
        }

        _breathVfx.Stop();
    }

    private void DealConeDamage()
    {
        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, _coneRange);
        Vector3 fwd = PlayerContext.transform.forward;

        foreach (var col in hits)
        {
            if (!col.TryGetComponent(out IDamageable target)) continue;

            Vector3 dir = col.transform.position - PlayerContext.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude == 0f) continue;
            if (Vector3.Angle(fwd, dir) > _coneAngle) continue;

            float dmg = _tickDamage * _tickRate;
            SkillDamageType type = SkillDamageType.Basic;
            
            if (!_burning.Contains(target))
            {
                PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
                _burning.Add(target);
            }

            target.ReceiveDamage(dmg, type);
        }
    }
}