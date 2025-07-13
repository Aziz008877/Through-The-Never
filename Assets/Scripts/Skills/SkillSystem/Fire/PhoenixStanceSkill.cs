using System.Collections;
using UnityEngine;

public class PhoenixStanceSkill : ActiveSkillBehaviour
{
    [Header("Shield stats")]
    [SerializeField] private int _maxHits = 4;
    [SerializeField] private float _duration = 5f;

    [Header("Explosion")]
    [SerializeField] private float _aoeRadius = 6f;
    [SerializeField] private float _aoeDamage = 40f;
    [SerializeField] private float _healPercent = .3f;

    [Header("VFX / SFX")]
    [SerializeField] private ParticleSystem _shieldVfx;
    [SerializeField] private ParticleSystem _explosionVfx;

    private int _hitsLeft;
    private float _totalDamageDone;
    private Coroutine _routine;

    public override void TryCast()
    {
        if (!IsReady || _routine != null) return;

        StartCooldown();
        _hitsLeft = _maxHits;
        _totalDamageDone = 0f;
        _routine = StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        if (_shieldVfx != null) _shieldVfx.Play();
        PlayerContext.PlayerHp.OnIncomingDamage += OnIncomingDamage;
        PlayerContext.PlayerState.ChangePlayerState(false);

        float timer = 0f;
        while (timer < _duration && _hitsLeft > 0)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        PlayerContext.PlayerHp.OnIncomingDamage -= OnIncomingDamage;
        if (_shieldVfx != null)
            _shieldVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        Explode();
        PlayerContext.PlayerState.ChangePlayerState(true);
        _routine = null;
    }

    private void OnIncomingDamage(ref float damage)
    {
        if (_hitsLeft <= 0) return;
        damage = 0f;
        _hitsLeft--;
    }

    private void Explode()
    {
        if (_explosionVfx != null)
        {
            _explosionVfx.transform.position = PlayerContext.transform.position;
            _explosionVfx.Play();
        }

        float radius = PlayerContext.SkillModifierHub.Apply(
            new SkillKey(Definition.Slot, SkillStat.Radius), _aoeRadius);

        Collider[] hits = Physics.OverlapSphere(
            PlayerContext.transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable enemy)) continue;

            float damage = _aoeDamage;
            SkillDamageType type = SkillDamageType.Basic;
            PlayerContext.ApplyDamageModifiers(ref damage, ref type);
            enemy.ReceiveDamage(damage, type);
            PlayerContext.FireOnDamageDealt(enemy, damage, type);
            _totalDamageDone += damage;
        }

        float heal = _totalDamageDone * _healPercent;
        PlayerContext.PlayerHp.ReceiveHP(heal);
    }
}
