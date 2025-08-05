using UnityEngine;
using Zenject;

public class FireCompanionController : CompanionControllerBase
{
    [Header("HP / логика")]
    [SerializeField] private float _maxHp = 100f;
    [SerializeField, Range(0f, 1f)] private float _defThreshold = 0.3f;
    [SerializeField] private float _globalCd = 0.25f;

    private float _hp;

    private void Start()
    {
        _hp = _maxHp;
        Ctx.Hp.OnIncomingDamage += OnHit;
    }

    private void OnDestroy()
    {
        Ctx.Hp.OnIncomingDamage -= OnHit;
    }

    private void OnHit(ref float dmg, IDamageable _)
    {
        if (dmg > 0f) _hp -= dmg;
    }

    protected override float GetDefenceThreshold() => _hp / _maxHp < _defThreshold ? 1f : 0f;
    protected override float GetGlobalCooldown() => _globalCd;
}