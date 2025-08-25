using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SkeletonAttack : BaseEnemyAttack, IBlindable
{
    private float _blindTimer;
    private float _missChance;
    private float _slowPercent;
    private float _dps;
    private NavMeshAgent _agent;
    private BaseEnemyHP  _hp; 
    private float _origSpeed;
    private Coroutine _blindRoutine;
    private ActorContext _attacker;
    private ActiveSkillBehaviour _sourceSkill;
    public bool IsBlinded() => _blindTimer > 0f;
    public float CurrentMissChance => IsBlinded() ? _missChance : 0f;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _hp = GetComponent<BaseEnemyHP>();
        _origSpeed = _agent.speed;
    }

    public void ApplyBlind(float duration, float missChance, float slowPercent, float dps, ActorContext attacker, ActiveSkillBehaviour sourceSkill = null)
    {
        _missChance = missChance;
        _slowPercent = Mathf.Clamp01(slowPercent);
        _dps = Mathf.Max(0f, dps);
        _attacker = attacker;
        _sourceSkill = sourceSkill;
        _blindTimer = Mathf.Max(_blindTimer, duration);
        _blindRoutine ??= StartCoroutine(BlindTick());
    }

    private IEnumerator BlindTick()
    {
        _agent.speed = _origSpeed * (1f - _slowPercent);

        while (_blindTimer > 0f)
        {
            float dt = Time.deltaTime;
            _blindTimer -= dt;

            if (_dps > 0f && _hp != null)
            {
                var def  = _sourceSkill ? _sourceSkill.Definition : null;
                var slot = def ? def.Slot : SkillSlot.Undefined;

                var ctx = new DamageContext
                {
                    Attacker = _attacker,
                    Target = _hp,
                    SkillBehaviour = _sourceSkill,
                    SkillDef = def,
                    Slot = slot,
                    Type = SkillDamageType.DOT,
                    Damage = _dps * dt,
                    IsCrit = false,
                    CritMultiplier = 1f,
                    SourceGO = gameObject
                };

                _hp.ReceiveDamage(ctx);
            }

            yield return null;
        }

        _agent.speed = _origSpeed;
        _blindRoutine = null;
    }
}
