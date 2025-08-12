using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TigrisOffenseSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private WaterJetEmitter _emitterPrefab;

    [Header("Extra Stats")]
    [SerializeField] private float _tickRate = 0.1f;
    [SerializeField] private float _pushForce = 20f;
    [SerializeField] private float _range = 8f;

    private Coroutine _channelCo;
    private WaterJetEmitter _emitter;
    private bool _isChanneling;

    public override void Inject(SkillDefinition definition, ActorContext context)
    {
        base.Inject(definition, context);

        if (context is PlayerContext pc)
        {
            pc.PlayerInput.OnSpecialSkillPressed += TryCast;
            pc.PlayerInput.OnSpecialSkillReleased += Release;
        }
    }

    public override void TryCast()
    {
        if (!IsReady || _isChanneling) return;
        base.TryCast();

        PlayerCtx?.Move.RotateTowardsMouse();

        _emitter = Instantiate(_emitterPrefab, Context.CastPivot.position, Context.CastPivot.rotation);
        _emitter.Bind(Context.CastPivot, _range);

        _isChanneling = true;
        _channelCo = StartCoroutine(ChannelRoutine());
    }

    private void Release()
    {
        if (!_isChanneling) return;
        _isChanneling = false;
    }

    private IEnumerator ChannelRoutine()
    {
        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);

        while (_isChanneling && elapsed < Duration)
        {
            DoTick();
            elapsed += _tickRate;
            yield return wait;
        }

        if (_emitter) Destroy(_emitter.gameObject);
        _emitter = null;

        _isChanneling = false;
        StartCooldown();
    }

    private void DoTick()
    {
        PlayerCtx?.Move.RotateTowardsMouse();

        Vector3 a = Context.CastPivot.position;
        Vector3 b = a + Context.CastPivot.forward * _range;

        var hits = Physics.OverlapCapsule(a, b, Radius, ~0, QueryTriggerInteraction.Collide);
        float dmgPerTick = (Damage) * _tickRate; // можно добавить доп. DPS если надо

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];

            if (col.TryGetComponent<IDamageable>(out var tgt))
            {
                float dmg = dmgPerTick;
                var type = SkillDamageType.Basic;
                Context.ApplyDamageModifiers(ref dmg, ref type);
                tgt.ReceiveDamage(dmg, type);
                Context.FireOnDamageDealt(tgt, dmg, type);
            }

            Vector3 push = Context.CastPivot.forward * _pushForce;

            if (col.attachedRigidbody != null)
                col.attachedRigidbody.AddForce(push, ForceMode.Acceleration);
            else if (col.TryGetComponent<NavMeshAgent>(out var agent) && agent.isOnNavMesh)
                agent.Move(push * Time.deltaTime);
            else
                col.transform.position += push * Time.deltaTime;
        }
    }
}
