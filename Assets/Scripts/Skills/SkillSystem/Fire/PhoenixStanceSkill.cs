using System;
using System.Collections;
using UnityEngine;

public class PhoenixStanceSkill : ActiveSkillBehaviour, IDefenceDurationSkill
{
    [Header("Shield stats")]
    [SerializeField] private int   _maxHits  = 4;
    [SerializeField] private float _duration = 5f;

    [Header("Explosion")]
    [SerializeField] private float _aoeRadius   = 6f;
    [SerializeField] private float _aoeDamage   = 40f;
    [SerializeField] private float _healPercent = .3f;

    [Header("VFX / SFX")]
    [SerializeField] private ParticleSystem _shieldVfx;
    [SerializeField] private ParticleSystem _explosionVfx;
    
    private int _hitsLeft;
    private float _totalDamageDone;
    private Coroutine _routine;
    public event Action OnDefenceStarted;
    public event Action OnDefenceFinished;
    public override void TryCast()
    {
        if (!IsReady || _routine != null) return;
        
        StartCooldown();
        _hitsLeft        = _maxHits;
        _totalDamageDone = 0f;
        _routine         = StartCoroutine(ShieldRoutine());

        Debug.Log($"<color=orange>[Phoenix Stance]</color> activated: {_maxHits} hits / {_duration}s");
    }

    private IEnumerator ShieldRoutine()
    {
        if (_shieldVfx) _shieldVfx.Play();

        Context.Hp.OnIncomingDamage += OnIncomingDamage;
        Context.State.ChangePlayerState(false);
        OnDefenceStarted?.Invoke();

        float timer = 0f;
        while (timer < _duration && _hitsLeft > 0)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        OnDefenceFinished?.Invoke();
        CleanupShield();
        Explode();
        _routine = null;
    }

    private void OnIncomingDamage(ref float damage, IDamageable source)
    {
        if (_hitsLeft <= 0) return;

        damage = 0f;
        _hitsLeft--;

        Debug.Log($"<color=orange>[Phoenix Stance]</color> blocked hit, left: {_hitsLeft}");

        if (_hitsLeft == 0)
        {
            StopCoroutine(_routine);
            CleanupShield();
            Explode();
            _routine = null;
        }
    }

    private void Explode()
    {
        if (_explosionVfx)
        {
            _explosionVfx.transform.position = Context.transform.position;
            _explosionVfx.Play();
        }

        float radius = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), _aoeRadius);

        Collider[] hits = Physics.OverlapSphere(Context.transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable enemy)) continue;

            float dmg  = _aoeDamage;
            SkillDamageType type = SkillDamageType.Basic;
            Context.ApplyDamageModifiers(ref dmg, ref type);

            enemy.ReceiveDamage(dmg, type);
            Context.FireOnDamageDealt(enemy, dmg, type);
            _totalDamageDone += dmg;
        }

        float heal = _totalDamageDone * _healPercent;
        Context.Hp.ReceiveHP(heal);

        /*Debug.Log($"<color=orange>[Phoenix Stance]</color> exploded for {_totalDamageDone:F0} " +
                  $"(heal {heal:F0})");*/
    }

    private void CleanupShield()
    {
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;
        Context.State.ChangePlayerState(true);
        if (_shieldVfx) _shieldVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        CleanupShield();
    }
}
