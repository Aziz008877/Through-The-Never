using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardAura : MonoBehaviour
{
    [SerializeField] private ParticleSystem _vfx;
    [SerializeField] private SphereCollider _sphere;

    private Transform _follow;
    private float _radius;
    private float _duration;
    private float _tickRate;
    private float _baseDamage;
    private int _frostMaxStacks;
    private float _maxSlow;
    private ActorContext _ctx;

    private readonly HashSet<Collider> _inside = new();

    public event Action OnFinished;

    public void Bind(Transform follow, float radius, float duration, float tickRate, float damage, int frostMaxStacks, float maxSlow, ActorContext ctx)
    {
        _follow = follow;
        _radius = radius;
        _duration = duration;
        _tickRate = tickRate;
        _baseDamage = damage;
        _frostMaxStacks = Mathf.Max(0, frostMaxStacks);
        _maxSlow = Mathf.Clamp01(maxSlow);
        _ctx = ctx;

        if (_sphere == null) _sphere = GetComponent<SphereCollider>();
        if (_sphere == null) _sphere = gameObject.AddComponent<SphereCollider>();
        _sphere.isTrigger = true;
        _sphere.radius = _radius;

        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(_follow, false);
        transform.localPosition = Vector3.zero;

        if (_vfx != null) _vfx.Play();
        StopAllCoroutines();
        StartCoroutine(Run());
    }


    private IEnumerator Run()
    {
        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);

        while (elapsed < _duration)
        {
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, _duration));
            float dmgMul = Mathf.Lerp(0.25f, 1f, t);
            float dmgTick = _baseDamage * dmgMul * _tickRate;
            float slowPerStack = (_frostMaxStacks > 0) ? (_maxSlow / _frostMaxStacks) : 0f;

            CleanupInside();

            // собираем уникальные цели по корневым компонентам
            var uniqueDamageables = new HashSet<IDamageable>();
            var uniqueFrosts = new HashSet<IFrostbiteReceivable>();

            foreach (var col in _inside)
            {
                if (col == null) continue;

                var dmgComp = col.GetComponentInParent<IDamageable>();
                if (dmgComp != null) uniqueDamageables.Add(dmgComp);

                var frostComp = col.GetComponentInParent<IFrostbiteReceivable>();
                if (frostComp != null) uniqueFrosts.Add(frostComp);
            }

            // урон
            foreach (var dmg in uniqueDamageables)
            {
                float d = dmgTick;
                var type = SkillDamageType.Basic;
                _ctx.ApplyDamageModifiers(ref d, ref type);
                dmg.ReceiveDamage(d, type);
                _ctx.FireOnDamageDealt(dmg, d, type);
            }

            // frostbite
            int targetStacks = Mathf.Clamp(Mathf.CeilToInt(t * _frostMaxStacks), 0, _frostMaxStacks);
            float refresh = _tickRate * 2f;

            foreach (var frost in uniqueFrosts)
            {
                if (targetStacks > frost.FrostStacks)
                {
                    int add = targetStacks - frost.FrostStacks;
                    for (int k = 0; k < add; k++)
                        frost.ApplyFrostbite(slowPerStack, 0f, refresh, _frostMaxStacks);
                }
                else if (frost.IsFrostActive)
                {
                    frost.ApplyFrostbite(slowPerStack, 0f, refresh, _frostMaxStacks);
                }
            }

            elapsed += _tickRate;
            yield return wait;
        }

        if (_vfx != null) _vfx.Stop();
        OnFinished?.Invoke();
        Destroy(gameObject);
    }

    private void CleanupInside()
    {
        _inside.RemoveWhere(c => c == null || (c.attachedRigidbody == null && c.transform == null));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_inside.Contains(other)) _inside.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_inside.Contains(other)) _inside.Remove(other);
    }
}
