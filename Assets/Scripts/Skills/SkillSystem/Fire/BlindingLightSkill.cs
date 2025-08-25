using System;
using System.Collections;
using UnityEngine;

public class BlindingLightSkill : ActiveSkillBehaviour, IDefenceDurationSkill
{
    [Header("Blinding Light Settings")]
    [SerializeField] private float _radius = 7f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _baseDps = 10f;
    [SerializeField] private float _missPercent = 0.5f;
    [SerializeField] private float _slowPercent = 0.5f;
    [SerializeField] private GameObject _sun;
    public event Action OnDefenceStarted;
    public event Action OnDefenceFinished;

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        StartCoroutine(BlindingRoutine());
        StartCooldown();
    }

    private IEnumerator BlindingRoutine()
    {
        OnDefenceStarted?.Invoke();
        _sun.gameObject.SetActive(true);
        
        float timer = 0f;
        while (timer < _duration)
        {
            ApplyBlindingEffect();
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        _sun.gameObject.SetActive(false);
        OnDefenceFinished?.Invoke();
    }

    private void ApplyBlindingEffect()
    {
        float radius = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), _radius);
        float dps = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), _baseDps);
        
        Collider[] hits = Physics.OverlapSphere(Context.transform.position, radius);
        foreach (var col in hits)
        {
            float dist = Vector3.Distance(Context.transform.position, col.transform.position);
            float proximity = Mathf.Clamp01(1f - dist / radius);

            if (col.TryGetComponent(out IBlindable enemy))
            {
                enemy.ApplyBlind(_duration, _missPercent, _slowPercent, Damage, Context);
            }

            if (col.TryGetComponent(out IDamageable dmg))
            {
                float tickDmg = dps * proximity;

                var ctx = new DamageContext
                {
                    Attacker = Context,
                    Target = dmg,
                    SkillBehaviour = this,
                    SkillDef = Definition,
                    Slot = Definition.Slot,
                    Type = SkillDamageType.Basic,
                    Damage = tickDmg,
                    IsCrit = false,
                    CritMultiplier = 1f,
                    SourceGO = gameObject
                };

                Context.ApplyDamageContextModifiers(ref ctx);
                dmg.ReceiveDamage(ctx);
            }
        }
    }
}