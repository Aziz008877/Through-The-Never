using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpireSkill : ActiveSkillBehaviour
{
    [Header("Core")]
    [SerializeField] private float _duration = 5f;
    [SerializeField] private float _cooldown = 15f;

    [Header("Totem Placement")]
    [SerializeField] private bool  _aimAtMouse = true;
    [SerializeField] private float _groundRayMax = 100f;
    [SerializeField] private LayerMask _groundMask = ~0;

    [Header("Aura")]
    [SerializeField] private float _radius = 6f;
    [SerializeField] private float _auraTick = 0.1f;
    [SerializeField] private float _pushForce = 12f;
    [SerializeField] private LayerMask _enemyMask = ~0;
    [SerializeField] private LayerMask _projectileMask = 0;

    [Header("Inside-Radius Buff")]
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private float _onHitKnockback = 10f;

    [Header("VFX")]
    [SerializeField] private GameObject _spirePrefab;
    [SerializeField] private ParticleSystem _spawnVfx;
    [SerializeField] private ParticleSystem _loopVfx;

    public override void TryCast()
    {
        if (!IsReady) return;

        Vector3 pos = GetSpawnPoint();
        var go = _spirePrefab ? Instantiate(_spirePrefab, pos, Quaternion.identity)
                              : new GameObject("IceSpire_Totem");
        if (!_spirePrefab)
        {
            go.transform.position = pos;
            var vis = go.AddComponent<SphereCollider>();
            vis.isTrigger = true; vis.radius = _radius * 0.1f;
        }

        var totem = go.GetComponent<IceSpireTotem>();
        if (!totem) totem = go.AddComponent<IceSpireTotem>();

        totem.Init(Context, this, _duration, _radius, _auraTick, _pushForce, _enemyMask, _projectileMask, _damageMultiplier, _onHitKnockback, _loopVfx);

        if (_spawnVfx)
        {
            _spawnVfx.transform.position = pos;
            _spawnVfx.Play();
        }

        StartCooldown();
    }

    private Vector3 GetSpawnPoint()
    {
        if (_aimAtMouse && Context.MainCamera)
        {
            var ray = Context.MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _groundRayMax, _groundMask, QueryTriggerInteraction.Ignore))
                return hit.point;
        }
        return Context.CastPivot ? Context.CastPivot.position : Context.transform.position;
    }
}

