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
        if (Vector3.Distance(PlayerContext.transform.position, center) <= radius)
            PlayerContext.PlayerHp.ReceiveHP(healTick);

        _enemies.Clear();
        Collider[] cols = Physics.OverlapSphere(center, radius);
        foreach (var col in cols)
            if (col.TryGetComponent(out IDamageable d) && !_enemies.Contains(d))
                _enemies.Add(d);

        SkillDamageType type = SkillDamageType.Basic;
        foreach (var enemy in _enemies)
        {
            float dmg = dmgTick;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);

            enemy.ReceiveDamage(dmg, type);
            PlayerContext.FireOnDamageDealt(enemy, dmg, type);
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
