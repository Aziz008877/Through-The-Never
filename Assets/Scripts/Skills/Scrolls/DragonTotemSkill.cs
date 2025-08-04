using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTotemSkill : ActiveSkillBehaviour
{
    private enum DragonState { Ice, Fire }

    [Header("Scene Objects")]
    [SerializeField] private GameObject _totemPrefab;
    [SerializeField] private ParticleSystem _iceWaveFx;
    [SerializeField] private ParticleSystem _fireWaveFx;
    [SerializeField] private ParticleSystem _explosionFx;

    [Header("Settings")]
    [SerializeField] private float _swapInterval = 2.5f;
    [SerializeField] private int _swapsTotal = 5;
    [SerializeField] private float _radius = 4f;
    [SerializeField] private float _fireDamage = 40f;

    private readonly List<IDamageable> _targets = new();

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
        var totem = _totemPrefab ? Instantiate(_totemPrefab, pos, Quaternion.identity) : null;
        DragonState state = DragonState.Ice;

        float pause = Mathf.Max(1f, _swapInterval);

        for (int i = 0; i < _swapsTotal; i++)
        {
            PlayWave(state, pos);
            yield return new WaitForSeconds(1f);
            _explosionFx.Play();
            
            yield return new WaitForSeconds(pause - 1f);

            state = state == DragonState.Ice ? DragonState.Fire : DragonState.Ice;
        }

        if (totem) Destroy(totem);
    }

    private void PlayWave(DragonState state, Vector3 center)
    {
        if (state == DragonState.Ice)
            _iceWaveFx.Play();
        else                           
            _fireWaveFx.Play();
        
        _targets.Clear();
        foreach (var col in Physics.OverlapSphere(center, _radius))
            if (col.TryGetComponent(out IDamageable d) && !_targets.Contains(d))
                _targets.Add(d);

        foreach (var tgt in _targets)
        {
            if (state == DragonState.Fire) DealFireDamage(tgt);
            else                           ApplyFreeze(tgt);
        }
    }

    private void DealFireDamage(IDamageable target)
    {
        float dmg = _fireDamage;
        SkillDamageType type = SkillDamageType.Basic;
        Context.ApplyDamageModifiers(ref dmg, ref type);

        target.ReceiveDamage(dmg, type);
        Context.FireOnDamageDealt(target, dmg, type);
    }

    private void ApplyFreeze(IDamageable target)
    {
        if (target is IFreezable f) f.ApplyFreeze(3f);
    }

    private bool GetAimPoint(out Vector3 point)
    {
        Ray ray = Camera.main.ViewportPointToRay(new(.5f, .5f));
        if (Physics.Raycast(ray, out var hit, Definition.Range > 0 ? Definition.Range : 30f,
                            Physics.AllLayers, QueryTriggerInteraction.Ignore))
        { point = hit.point; return true; }

        point = Vector3.zero; return false;
    }
}

public interface IFreezable { void ApplyFreeze(float seconds); }
