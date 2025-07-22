using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTotemSkill : ActiveSkillBehaviour
{
    private enum DragonState { Ice, Fire }

    [Header("Prefabs / FX")]
    [SerializeField] private GameObject _totemPrefab;
    [SerializeField] private GameObject _iceWavePrefab;
    [SerializeField] private GameObject _fireWavePrefab;
    [SerializeField] private GameObject _explosionPrefab;

    [Header("Logic")]
    [SerializeField] private float _swapInterval = 2.5f;
    [SerializeField] private int   _swapsTotal = 5;
    [SerializeField] private float _waveRadius = 4f;
    [SerializeField] private float _fireDamage = 40f;
    [SerializeField] private LayerMask _enemyMask;

    private readonly List<IDamageable> _buffer = new();

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        if (!GetAimPoint(out Vector3 point)) return;

        StartCoroutine(TotemRoutine(point));
        StartCooldown();
    }

    private IEnumerator TotemRoutine(Vector3 pos)
    {
        GameObject totem = _totemPrefab ? Instantiate(_totemPrefab, pos, Quaternion.identity) : null;

        DragonState state = DragonState.Ice;

        for (int i = 0; i < _swapsTotal; i++)
        {
            yield return new WaitForSeconds(1f);
            
            SpawnWave(state, pos);
            if (_explosionPrefab) Instantiate(_explosionPrefab, pos, Quaternion.identity);
            state = state == DragonState.Ice ? DragonState.Fire : DragonState.Ice;
        }

        if (totem) Destroy(totem);
    }

    private void SpawnWave(DragonState state, Vector3 center)
    {
        if (state == DragonState.Ice && _iceWavePrefab)
            Instantiate(_iceWavePrefab, center, Quaternion.identity)
                     .transform.localScale = Vector3.one * _waveRadius;
        if (state == DragonState.Fire && _fireWavePrefab)
            Instantiate(_fireWavePrefab, center, Quaternion.identity)
                     .transform.localScale = Vector3.one * _waveRadius;
        
        _buffer.Clear();
        Collider[] cols = Physics.OverlapSphere(center, _waveRadius, _enemyMask);
        foreach (var col in cols)
            if (col.TryGetComponent(out IDamageable d) && !_buffer.Contains(d))
                _buffer.Add(d);

        foreach (var tgt in _buffer)
        {
            if (state == DragonState.Fire)
                DealFireDamage(tgt);
            else
                ApplyFreeze(tgt);
        }
    }

    private void DealFireDamage(IDamageable target)
    {
        float dmg = _fireDamage;
        SkillDamageType type = SkillDamageType.Basic;
        PlayerContext.ApplyDamageModifiers(ref dmg, ref type);

        target.ReceiveDamage(dmg, type);
        PlayerContext.FireOnDamageDealt(target, dmg, type);
    }

    private void ApplyFreeze(IDamageable target)
    {
        if (target is IFreezable freezable)
            freezable.ApplyFreeze(3f);
    }

    private bool GetAimPoint(out Vector3 point)
    {
        Ray ray = Camera.main.ViewportPointToRay(new(.5f, .5f));
        if (Physics.Raycast(ray, out var hit,
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


public interface IFreezable
{
    void ApplyFreeze(float seconds);
}
