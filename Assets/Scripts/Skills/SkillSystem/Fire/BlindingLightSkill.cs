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
        base.TryCast();

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
        float radius = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), _radius);
        float dps = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), _baseDps);
        
        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, radius);
        foreach (var col in hits)
        {
            float dist = Vector3.Distance(PlayerContext.transform.position, col.transform.position);
            float proximity = Mathf.Clamp01(1f - dist / radius);

            if (col.TryGetComponent(out IBlindable enemy))
            {
                enemy.ApplyBlind(_duration, 0.5f, 0.5f, dps * proximity);
            }

            if (col.TryGetComponent(out IDamageable dmg))
            {
                float tickDmg = dps * proximity;
                SkillDamageType type = SkillDamageType.Basic;
                PlayerContext.ApplyDamageModifiers(ref tickDmg, ref type);
                dmg.ReceiveDamage(tickDmg, type);
                PlayerContext.FireOnDamageDealt(dmg, tickDmg, type);
            }
        }
    }
}