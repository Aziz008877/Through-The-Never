using UnityEngine;

public class FireCompanionController : CompanionControllerBase
{
    [Header("HP")]
    [SerializeField] float _maxHp = 100f;
    [SerializeField, Range(0f, 1f)] float _defThreshold = 0.3f;

    [Header("Skills")]
    [SerializeField] PhoenixStanceSkill  _defensive;      // Shield
    [SerializeField] FirenadoSkill       _special;        // AoE
    [SerializeField] PlayerDashSkill     _dash;           // Dash (Trailblazer)
    [SerializeField] ActiveSkillBehaviour _basic;         // ← базовый (FireballSkill)

    [Header("Logic")]
    [SerializeField] float _globalCd = 0.25f;

    float _hp;
    float _nextCast;

    void Start() => _hp = _maxHp;

    void OnEnable()  => Ctx.Hp.OnIncomingDamage += OnHit;
    void OnDisable() => Ctx.Hp.OnIncomingDamage -= OnHit;
    void OnHit(ref float dmg, IDamageable _) { if (dmg > 0f) _hp -= dmg; }

    protected override void CombatLoop()
    {
        /*if (Time.time < _nextCast) return;

        /* 1 — Phoenix Stance #1#
        if (_hp / _maxHp < _defThreshold && _defensive.IsReady)
        { _defensive.TryCast(); SetCd(); return; }

        /* 2 — Firenado #1#
        if (_special.IsReady)
        { _special.TryCast(); SetCd(); return; }

        /* 3 — Dash во врага #1#
        if (_dash.IsReady)
        {
            Transform t = NearestEnemy();
            if (t) { _dash.TryCastAtTarget(t); SetCd(); return; }
        }

        /* 4 — Базовый выстрел #1#
        if (_basic.IsReady)
        {
            if (PlayerIsActive)
                _basic.TryCastAtPoint(LastShotPoint);      // дублируем цель игрока
            else
            {
                Transform t = NearestEnemy();
                if (t) _basic.TryCastAtTarget(t);
            }
            SetCd();
        }*/
    }
    void SetCd() => _nextCast = Time.time + _globalCd;
}