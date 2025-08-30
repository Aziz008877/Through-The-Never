using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IceSpikesSkill : ActiveSkillBehaviour
{
    [Header("VFX / SFX")]
    [SerializeField] private GameObject _spikeWavePrefab;

    [Header("Logic")]
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _waveCount = 4;
    [SerializeField] private float _radiusGrowth  = 1f;
    private readonly List<IDamageable> _targets = new();
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        if (!GetAimPoint(out Vector3 point)) return;

        StartCoroutine(SpikeRoutine(point));
        StartCooldown();
    }

    private IEnumerator SpikeRoutine(Vector3 center)
    {
        float radius = Definition.Raduis > 0f ? Definition.Raduis : 2f;
        float totalDmg = Definition.Damage > 0f ? Definition.Damage : 40f;
        float dmgPerWave = totalDmg / _waveCount;

        for (int wave = 0; wave < _waveCount; wave++)
        {
            SpawnWave(center, radius, dmgPerWave);

            radius += _radiusGrowth;
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnWave(Vector3 center, float radius, float dmg)
    {
        if (_spikeWavePrefab)
            Instantiate(_spikeWavePrefab, center, Quaternion.identity)
                .transform.localScale = Vector3.one * radius;

        _targets.Clear();
        var hits = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out IDamageable t) && !_targets.Contains(t))
                _targets.Add(t);
        }
        if (_targets.Count == 0) return;

        for (int i = 0; i < _targets.Count; i++)
        {
            var target = _targets[i];
            var ctx = BuildDamage(dmg, SkillDamageType.Basic, center, Vector3.up, gameObject);
            ctx.Target = target;
            
            Context.ApplyDamageContextModifiers(ref ctx);
            target.ReceiveDamage(ctx);
        }
    }


    private bool GetAimPoint(out Vector3 point)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
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
