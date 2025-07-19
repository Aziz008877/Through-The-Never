using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LavaPoolArea : MonoBehaviour
{
    private float _dps;
    private float _lifeTime;
    private PlayerContext _ctx;
    private readonly HashSet<IDamageable> _inside = new();
    public void Init(float dps, float radius, float lifeTime, PlayerContext ctx)
    {
        _dps = dps;
        _lifeTime = lifeTime;
        _ctx = ctx;

        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;

        Destroy(gameObject, _lifeTime);
    }
    private void OnTriggerEnter(Collider other) => TryAdd(other);
    private void OnTriggerStay (Collider other) => TryAdd(other);
    private void OnTriggerExit (Collider other)
    {
        if (other.TryGetComponent(out IDamageable d)) _inside.Remove(d);
    }

    private void TryAdd(Collider col)
    {
        if (!col.TryGetComponent(out IDamageable d)) return;
        _inside.Add(d);
    }

    private float _tickTimer;
    private void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer < 0.5f) return;
        _tickTimer = 0f;

        foreach (var tgt in _inside)
        {
            float tick = _dps * 0.5f;
            SkillDamageType type = SkillDamageType.Basic;

            _ctx.ApplyDamageModifiers(ref tick, ref type);
            tgt.ReceiveDamage(tick, type);

            if (tgt is IDotReceivable dot)
                dot.ApplyDot(_dps, _lifeTime);
        }
    }
}