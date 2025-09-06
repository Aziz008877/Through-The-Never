using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TigrisOffenseSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private WaterJetEmitter _emitterPrefab;

    [Header("Stats")]
    [SerializeField] private float _tickRate  = 0.1f;  // частота тиков урона/толкания
    [SerializeField] private float _pushForce = 20f;    // сила отталкивания
    [SerializeField] private float _range     = 8f;     // длина струи
    [SerializeField] private float _duration  = 5f;     // ДОЛЖНО БЫТЬ 5 сек

    private Coroutine _routine;
    private WaterJetEmitter _emitter;

    public override void Inject(SkillDefinition definition, ActorContext context)
    {
        base.Inject(definition, context);

        // Только нажатие — без Release и без тумблера
        if (context is PlayerContext pc)
            pc.PlayerInput.OnSpecialSkillPressed += TryCast;
    }

    public override void TryCast()
    {
        if (!IsReady || _routine != null) return;

        base.TryCast();            // НЕ ставит КД — мы поставим по окончании действия

        PlayerCtx?.Move.RotateTowardsMouse();

        // Спавним и привязываем эмиттер к CastPivot
        _emitter = Instantiate(_emitterPrefab, Context.CastPivot.position, Context.CastPivot.rotation);
        _emitter.Bind(Context.CastPivot, _range);

        // Форсим луп у всех партиклов эмиттера (на случай, если в префабе не включено)
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

        // КД берём из Definition (или переопредели StartCooldown(x) если нужно фикс. значение)
        StartCooldown();
    }

    private void DoTick()
    {
        // Подворачиваемся к мыши для приятного контроля
        PlayerCtx?.Move.RotateTowardsMouse();

        Vector3 a = Context.CastPivot.position;
        Vector3 b = a + Context.CastPivot.forward * _range;

        // Радиус берём из базового скилла (если у тебя это поле/проперти от хаба)
        float radius = Radius;

        var hits = Physics.OverlapCapsule(a, b, radius, ~0, QueryTriggerInteraction.Collide);
        float dmgPerTick = Damage * _tickRate;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];

            // Урон
            if (col.TryGetComponent<IDamageable>(out var tgt))
            {
                var ctx = BuildDamage(
                    dmgPerTick,
                    SkillDamageType.Basic,
                    hitPoint: col.transform.position,
                    hitNormal: Vector3.up,
                    sourceGO: gameObject
                );
                ctx.Target = tgt;

                Context.ApplyDamageContextModifiers(ref ctx);
                tgt.ReceiveDamage(ctx);
            }

            // Отталкивание
            Vector3 push = Context.CastPivot.forward * _pushForce;

            if (col.attachedRigidbody != null)
            {
                col.attachedRigidbody.AddForce(push, ForceMode.Acceleration);
            }
            else if (col.TryGetComponent<NavMeshAgent>(out var agent) && agent.isOnNavMesh)
            {
                agent.Move(push * Time.deltaTime);
            }
            else
            {
                col.transform.position += push * Time.deltaTime;
            }
        }
    }

    private void CleanupEmitter()
    {
        if (_emitter)
        {
            // Можно остановить партиклы мягко, но раз эффект закончился — просто уничтожаем
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
        // ВАЖНО: при выгрузке/смерти без постановки КД
    }

    private static void ForceLoopParticles(GameObject root)
    {
        if (!root) return;
        var pss = root.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < pss.Length; i++)
        {
            var ps = pss[i];
            var main = ps.main;
            main.loop = true;          // форсим луп
            if (!ps.isPlaying) ps.Play();
        }
    }
}
