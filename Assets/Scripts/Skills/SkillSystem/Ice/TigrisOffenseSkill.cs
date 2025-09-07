using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TigrisOffenseSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private WaterJetEmitter _emitterPrefab;

    [Header("Stats")]
    [SerializeField] private float _tickRate  = 0.1f;
    [SerializeField] private float _pushForce = 20f;
    [SerializeField] private float _range = 8f;
    [SerializeField] private float _duration  = 5f;

    private Coroutine _routine;
    private WaterJetEmitter _emitter;
    private readonly HashSet<IDamageable> _hitThisTick = new();
    public override void Inject(SkillDefinition definition, ActorContext context)
    {
        base.Inject(definition, context);
        
        if (context is PlayerContext pc)
            pc.PlayerInput.OnSpecialSkillPressed += TryCast;
    }

    public override void TryCast()
    {
        if (!IsReady || _routine != null) return;

        base.TryCast();

        PlayerCtx?.Move.RotateTowardsMouse();
        
        _emitter = Instantiate(_emitterPrefab, Context.CastPivot.position, Context.CastPivot.rotation);
        _emitter.Bind(Context.CastPivot, _range);
        
        ForceLoopParticles(_emitter.gameObject);

        _routine = StartCoroutine(EffectRoutine());
    }

    private IEnumerator EffectRoutine()
    {
        float elapsed = 0f;
        var wait = new WaitForSeconds(Mathf.Max(0.01f, _tickRate));

        while (elapsed < _duration)
        {
            DoTick();
            elapsed += _tickRate;
            yield return wait;
        }

        CleanupEmitter();
        _routine = null;
        
        StartCooldown();
    }

    private void DoTick()
    {
        PlayerCtx?.Move.RotateTowardsMouse();

        Vector3 a = Context.CastPivot.position;
        Vector3 b = a + Context.CastPivot.forward * _range;
        float radius = Radius;
        
        var hits = Physics.OverlapCapsule(a, b, radius, ~0, QueryTriggerInteraction.Ignore);

        float dmgPerTick = Damage * _tickRate;
        _hitThisTick.Clear();

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col.transform.IsChildOf(Context.transform)) continue;
            
            var targetComp = col.GetComponentInParent<Component>();
            var dmg = targetComp ? targetComp.GetComponent<IDamageable>() : null;
            if (dmg == null || !dmg.CanBeDamaged) continue;
            
            if (!_hitThisTick.Add(dmg)) continue;
            
            var ctx = BuildDamage(
                dmgPerTick,
                SkillDamageType.Basic,
                hitPoint: col.transform.position,
                hitNormal: Vector3.up,
                sourceGO: gameObject
            );
            ctx.Target = dmg;

            Context.ApplyDamageContextModifiers(ref ctx);
            dmg.ReceiveDamage(ctx);
            
            Vector3 push = Context.CastPivot.forward * _pushForce;
            
            var root = (dmg as Component)?.transform;
            if (!root) continue;

            if (root.TryGetComponent<Rigidbody>(out var rb) && !rb.isKinematic)
            {
                rb.AddForce(push, ForceMode.Acceleration);
            }
            else if (root.TryGetComponent<NavMeshAgent>(out var agent) && agent.isOnNavMesh)
            {
                agent.Move(push * Time.deltaTime);
            }
            else
            {
                root.position += push * Time.deltaTime;
            }
        } 
    }

    private void CleanupEmitter()
    {
        if (_emitter)
        {
            Destroy(_emitter.gameObject);
            _emitter = null;
        }
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        CleanupEmitter();
    }

    private static void ForceLoopParticles(GameObject root)
    {
        if (!root) return;
        var pss = root.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < pss.Length; i++)
        {
            var ps = pss[i];
            var main = ps.main;
            main.loop = true; 
            if (!ps.isPlaying) ps.Play();
        }
    }
}
