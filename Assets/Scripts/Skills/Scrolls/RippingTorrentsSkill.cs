using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippingTorrentsSkill : ActiveSkillBehaviour
{
    [Header("Prefabs  &  SFX")] [SerializeField]
    private GameObject _geyserPrefab;

    [SerializeField] private GameObject _puddlePrefab;

    [Header("Logic")] [SerializeField] private float _radius = 6f;
    [SerializeField] private float _spawnInterval = .6f;
    [SerializeField] private int _wavesTotal = 8;
    [SerializeField] private float _geyserDamage = 25f;
    [SerializeField] private float _healAmount = 10f;
    [SerializeField] private float _puddleLife = 3f;
    private readonly List<IDamageable> _buffer = new();
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        if (!GetAimPoint(out Vector3 center)) return;

        StartCoroutine(GeyserRoutine(center));
        StartCooldown();
    }

    private IEnumerator GeyserRoutine(Vector3 center)
    {
        int wavesDone = 0;
        WaitForSeconds wait = new(_spawnInterval);

        while (wavesDone < _wavesTotal)
        {
            SpawnGeyser(center);
            wavesDone++;
            yield return wait;
        }
    }

    private void SpawnGeyser(Vector3 center)
    {
        Vector2 rnd = Random.insideUnitCircle * _radius;
        Vector3 pos = center + new Vector3(rnd.x, 0, rnd.y);

        if (_geyserPrefab)
            Instantiate(_geyserPrefab, pos, Quaternion.identity);

        DealDamage(pos, 1.5f);
        
        if (_puddlePrefab)
            StartCoroutine(PuddleRoutine(
                Instantiate(_puddlePrefab, pos, Quaternion.identity), pos));
    }

    private void DealDamage(Vector3 pos, float radius)
    {
        _buffer.Clear();
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (var col in hits)
            if (col.TryGetComponent(out IDamageable d) && !_buffer.Contains(d))
                _buffer.Add(d);

        SkillDamageType type = SkillDamageType.Basic;
        foreach (var tgt in _buffer)
        {
            float dmg = _geyserDamage;
            Context.ApplyDamageModifiers(ref dmg, ref type);

            tgt.ReceiveDamage(dmg, type);
            Context.FireOnDamageDealt(tgt, dmg, type);
        }
    }

    private IEnumerator PuddleRoutine(GameObject puddle, Vector3 pos)
    {
        float life = _puddleLife;
        bool healed = false;
        while (life > 0f)
        {
            life -= Time.deltaTime;

            if (!healed && Vector3.Distance(Context.transform.position, pos) < 1.5f)
            {
                Context.Hp.ReceiveHP(_healAmount);
                healed = true;
            }

            yield return null;
        }

        if (puddle) Destroy(puddle);
    }

    private bool GetAimPoint(out Vector3 point)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new(.5f, .5f));
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
