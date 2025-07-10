using UnityEngine;

public class FirebeamSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castVfx;
    [SerializeField] private ParticleSystem _beamVfx;
    [SerializeField] private ParticleSystem _hitVfx;

    [Header("Damage / Timings")]
    [SerializeField] private float _baseDps  = 4f;
    [SerializeField] private float _maxDps   = 16f;
    [SerializeField] private float _tickRate = .25f;

    private float _lifeTimer;
    private float _tickTimer;
    private bool _active;

    public override void TryCast()
    {
        if (!IsReady || _active) return;

        transform.SetParent(null, true);

        _lifeTimer = 0f;
        _tickTimer = 0f;
        _active    = true;

        _castVfx.Play();
        _beamVfx.Play();

        StartCooldown();
    }

    private void Update()
    {
        if (!_active) return;
        
        transform.position = PlayerContext.PlayerPosition.position;

        float range = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Range), Definition.Range);

        IDamageable target = FindClosest(range);

        Vector3 dir = PlayerContext.transform.forward;
        
        if (target != null)
        {
            dir = ((MonoBehaviour)target).transform.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir.normalized);
        }

        Vector3 start = transform.position;
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(start, transform.forward, out hitInfo, range);
        Vector3 end = hit ? hitInfo.point : start + transform.forward * range;
        
        if (hit)
        {
            _hitVfx.transform.position = end;
            if (!_hitVfx.isPlaying) _hitVfx.Play();
        }
        else if (_hitVfx.isPlaying) _hitVfx.Stop();
        
        Vector3 s = _beamVfx.transform.localScale;
        _beamVfx.transform.localScale =
            new Vector3(s.x, s.y, Vector3.Distance(start, end));
        
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= _tickRate)
        {
            _tickTimer -= _tickRate;

            if (hit && hitInfo.collider.TryGetComponent(out IDamageable d))
            {
                float t   = _lifeTimer / Definition.Duration;
                float dps = Mathf.Lerp(_baseDps, _maxDps, t);
                float dmg = dps * _tickRate;

                SkillDamageType type = SkillDamageType.Basic;
                PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
                d.ReceiveDamage(dmg, type);
                PlayerContext.FireOnDamageDealt(d, dmg, type);
            }
        }
        
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= Definition.Duration) StopBeam();
    }

    private void StopBeam()
    {
        if (!_active) return;
        _active = false;
        _castVfx.Stop();
        _beamVfx.Stop();
        _hitVfx.Stop();
    }

    private IDamageable FindClosest(float range)
    {
        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, range);
        IDamageable best = null;
        float bestSqr = float.MaxValue;
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable d)) continue;
            float sqr = (h.transform.position - PlayerContext.transform.position).sqrMagnitude;
            if (sqr < bestSqr) { best = d; bestSqr = sqr; }
        }
        return best;
    }

    private void OnDisable() => StopBeam();
}
