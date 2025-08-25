using System.Collections.Generic;
using UnityEngine;

public class DeepfrostWaterProjectile : IceBasicAttackprojectile
{
    [SerializeField] private float _range = 15f;
    [SerializeField] private float _rayRadius = 0.5f;
    [SerializeField] private float _tickRate = 0.1f;
    [SerializeField] private float _dpsMultiplier = 1.0f;
    [SerializeField] private float _freezeRequired = 2f;
    [SerializeField] private float _freezeDuration = 2f;
    [SerializeField] private float _grace = 0.12f;

    private struct TargetState
    {
        public float timeUnder;
        public float nextTick;
        public float lastSeenTime;
        public bool frozen;
    }

    private readonly Dictionary<IDamageable, TargetState> _targets = new();
    private readonly HashSet<IDamageable> _seenThisFrame = new();
    private readonly List<IDamageable> _toCull = new();
    private readonly List<IDamageable> _keysBuffer = new();

    private Transform _body;
    private Vector3 _baseScale;

    private void Awake()
    {
        _body = _flightVfx ? _flightVfx.transform : null;
        if (_body) _baseScale = _body.localScale;
    }

    private void Update()
    {
        if (_context == null) return;

        Vector3 origin = _context.CastPivot ? _context.CastPivot.position : transform.position;
        Vector3 dir    = _context.CastPivot ? _context.CastPivot.forward  : transform.forward;

        float maxLen = _range;
        if (Physics.Raycast(origin, dir, out var wallHit, _range))
            maxLen = wallHit.distance;

        transform.SetPositionAndRotation(origin, Quaternion.LookRotation(dir, Vector3.up));

        if (_flightVfx)
        {
            if (!_flightVfx.isPlaying) _flightVfx.Play();
            _body.localPosition = Vector3.zero;
            _body.localScale = new Vector3(_baseScale.x, _baseScale.y, _baseScale.z * maxLen);
        }

        var hits = Physics.SphereCastAll(origin, _rayRadius, dir, maxLen);

        float now = Time.time;
        _seenThisFrame.Clear();

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i].collider;
            if (!col) continue;

            var dmg = col.GetComponentInParent<IDamageable>();
            if (dmg == null) continue;

            _seenThisFrame.Add(dmg);
            _targets.TryGetValue(dmg, out var st);

            st.lastSeenTime = now;
            st.timeUnder += Time.deltaTime;

            if (st.nextTick <= 0f)
            {
                float dps        = _instantDamage * _dpsMultiplier;
                float tickDamage = dps * Mathf.Max(0.01f, _tickRate);
            
                var ctx = new DamageContext
                {
                    Attacker       = _context,                         // ActorContext источника
                    Target         = dmg,
                    SkillBehaviour = null,                              // не активный скилл
                    SkillDef       = null,
                    Slot           = SkillSlot.Special,
                    Type           = _damageType,                       // как у тебя было (Basic/DOT и т.д.)
                    Damage         = tickDamage,
                    IsCrit         = false,
                    CritMultiplier = 1f,
                    HitPoint       = (dmg as Component)?.transform.position ?? transform.position,
                    HitNormal      = Vector3.up,
                    SourceGO       = gameObject
                };
            
                _context.ApplyDamageContextModifiers(ref ctx);
                dmg.ReceiveDamage(ctx);
            
                st.nextTick = _tickRate;
            }

            else
            {
                st.nextTick -= Time.deltaTime;
            }

            if (!st.frozen && st.timeUnder >= _freezeRequired)
            {
                var stun = col.GetComponent<StunDebuff>();
                if (stun != null)
                {
                    stun.ApplyStun(_freezeDuration);
                    st.frozen = true;
                }
            }

            _targets[dmg] = st;
        }

        if (_targets.Count > 0)
        {
            _toCull.Clear();
            _keysBuffer.Clear();
            foreach (var k in _targets.Keys) _keysBuffer.Add(k);

            for (int i = 0; i < _keysBuffer.Count; i++)
            {
                var key = _keysBuffer[i];
                var st = _targets[key];

                if (_seenThisFrame.Contains(key)) continue;

                if (now - st.lastSeenTime > _grace)
                {
                    st.timeUnder = 0f;
                    st.frozen = false;
                    st.nextTick = 0f;
                    if (now - st.lastSeenTime > 1f) _toCull.Add(key);
                }
                _targets[key] = st;
            }

            for (int i = 0; i < _toCull.Count; i++) _targets.Remove(_toCull[i]);
            _toCull.Clear();
        }
    }
}
