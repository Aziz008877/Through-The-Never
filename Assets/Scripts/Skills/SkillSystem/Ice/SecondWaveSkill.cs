using System.Collections.Generic;
using UnityEngine;
public class SecondWaveSkill : PassiveSkillBehaviour
{
    [SerializeField] private float _sourceCooldown = 1.0f;
    [SerializeField] private ParticleSystem _waveVfx;
    private readonly List<StunDebuff> _subs = new();
    private readonly Dictionary<StunDebuff, float> _nextAllowed = new();

    public override void EnablePassive()
    {
        foreach (var enemy in Context.EnemyHandler.Enemies)
        {
            if (enemy.TryGetComponent(out StunDebuff stunDebuff))
            {
                stunDebuff.OnStunStarted += OnEnemyStunned;
                _subs.Add(stunDebuff);
            }
        }
    }

    public override void DisablePassive()
    {
        foreach (var stunDebuff in _subs)
        {
            if (stunDebuff) stunDebuff.OnStunStarted -= OnEnemyStunned;
        }
        
        _subs.Clear();
        _nextAllowed.Clear();
    }

    private void OnEnemyStunned(StunDebuff debuff, float duration)
    {
        if (!debuff) return;
        float now = Time.time;
        if (_nextAllowed.TryGetValue(debuff, out float t) && now < t) return;
        _nextAllowed[debuff] = now + _sourceCooldown;

        if (_waveVfx)
        {
            _waveVfx.transform.position = debuff.transform.position;
            _waveVfx.Play();
        }

        var hits = Physics.OverlapSphere(debuff.transform.position, Definition.Raduis);

        IDamageable self = debuff.GetComponent<IDamageable>();
        
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            var dmg = col.GetComponent<IDamageable>();
            if (dmg == null || dmg == self) continue;

            float dealt = Definition.Damage;
            var type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dealt, ref type);
            dmg.ReceiveDamage(dealt, type);
            Context.FireOnDamageDealt(dmg, dealt, type);
        }
    }
}
