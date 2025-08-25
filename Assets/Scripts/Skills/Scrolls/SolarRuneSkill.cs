using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarRuneSkill : ActiveSkillBehaviour
{
    [Header("VFX / SFX")]
    [SerializeField] private GameObject _runePrefab;

    [Header("Logic")]
    [SerializeField] private float _tickRate = 0.5f;
    [SerializeField] private float _baseDps = 10f;
    [SerializeField] private float _baseHps = 5f;
    [SerializeField] private float _radius = 4f;
    private readonly List<IDamageable> _enemies = new();

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        if (!GetAimPoint(out Vector3 point)) return;

        StartCoroutine(RuneRoutine(point));
        StartCooldown();
    }

    private IEnumerator RuneRoutine(Vector3 center)
    {
        if (_runePrefab)
            Instantiate(_runePrefab, center, Quaternion.identity);

        float lifeTime = Definition.Duration > 0f ? Definition.Duration : 10f;
        float radius   = Definition.Raduis > 0f ? Definition.Raduis : _radius;

        float dmgPerTick  = (_baseDps * _tickRate);
        float healPerTick = (_baseHps * _tickRate);

        WaitForSeconds wait = new(_tickRate);
        float elapsed = 0f;

        while (elapsed < lifeTime)
        {
            Tick(center, radius, dmgPerTick, healPerTick);
            elapsed += _tickRate;
            yield return wait;
        }
    }

    private void Tick(Vector3 center, float radius, float dmgTick, float healTick)
    {
        // Лечение себе (без контекста урона — это не урон)
        if (Vector3.Distance(Context.transform.position, center) <= radius)
            Context.Hp.ReceiveHP(healTick);

        _enemies.Clear();
        var cols = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].TryGetComponent(out IDamageable d) && !_enemies.Contains(d))
                _enemies.Add(d);
        }
        if (_enemies.Count == 0) return;

        for (int i = 0; i < _enemies.Count; i++)
        {
            var enemy = _enemies[i];

            // Собираем фактический удар тиком
            var ctx = BuildDamage(dmgTick, SkillDamageType.Basic, center, Vector3.up, gameObject);
            ctx.Target = enemy;

            // Если нужно — можно ещё раз прогнать контекстные модификаторы:
            // Context.ApplyDamageContextModifiers(ref ctx);

            enemy.ReceiveDamage(ctx); // события разойдутся из цели автоматически
        }
    }

    private bool GetAimPoint(out Vector3 point)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit,
                Definition.Range > 0 ? Definition.Range : 30f,
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
