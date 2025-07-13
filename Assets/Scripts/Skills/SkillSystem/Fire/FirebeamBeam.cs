using UnityEngine;
public class FirebeamBeam : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem _castVfx;
    [SerializeField] private ParticleSystem _beamVfx;
    [SerializeField] private ParticleSystem _hitVfx;
    [SerializeField] private ParticleSystem _mainVfx;
    
    private PlayerContext _playerContext;
    private float _lifeTime;
    private float _range;
    private float _tickRate;
    private float _baseDps;
    private float _maxDps;
    
    private float _lifeTimer;
    private float _tickTimer;
    private bool _running;
    private Vector3 _baseScale;
    
    public void Init(PlayerContext ctx, float duration, float range, float tickRate, float baseDps, float maxDps)
    {
        _baseScale = _beamVfx.transform.localScale;
        _playerContext = ctx;
        _lifeTime = duration;
        _range = range;
        _tickRate = tickRate;
        _baseDps = baseDps;
        _maxDps = maxDps;

        _running = true;
        _lifeTimer = 0f;
        _tickTimer = 0f;

        _castVfx.Play();
        _beamVfx.Play();
        _mainVfx.Play();
    }
    
    private void Update()
    {
        if (!_running) return;

        var playerPos = _playerContext.PlayerPosition.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + 2, playerPos.z);

        IDamageable target = FindClosest(_range);

        Vector3 dir = _playerContext.transform.forward;
        if (target != null)
        {
            dir = ((MonoBehaviour)target).transform.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir.normalized);
        }

        Vector3 start = transform.position;
        bool hit = Physics.Raycast(start, transform.forward, out var hitInfo, _range);
        Vector3 end = hit ? hitInfo.point : start + transform.forward * _range;

        HandleVfx(hit, end, start);
        
        _tickTimer += Time.deltaTime;
        
        if (_tickTimer >= _tickRate)
        {
            _tickTimer -= _tickRate;

            if (hit && hitInfo.collider.TryGetComponent(out IDamageable d))
            {
                float t = Mathf.Clamp01(_lifeTimer / _lifeTime);
                float dps = Mathf.Lerp(_baseDps, _maxDps, t);
                float dmg = dps * _tickRate;

                SkillDamageType type = SkillDamageType.Basic;
                _playerContext.ApplyDamageModifiers(ref dmg, ref type);
                d.ReceiveDamage(dmg, type);
                _playerContext.FireOnDamageDealt(d, dmg, type);
            }
        }

        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime) Stop();
    }
    
    private void HandleVfx(bool hit, Vector3 end, Vector3 start)
    {
        if (hit)
        {
            _hitVfx.transform.position = end;
            if (!_hitVfx.isPlaying) _hitVfx.Play();
        }
        else if (_hitVfx.isPlaying) _hitVfx.Stop();

        /*Vector3 s = _beamVfx.transform.localScale;
        float len = Vector3.Distance(start, end);
        _beamVfx.transform.localScale = new Vector3(s.x, s.y, len);*/
        
        float lenWS = Vector3.Distance(start, end);
        float parentScaleZ = transform.lossyScale.z;
        float lenLS = lenWS / parentScaleZ;

        _beamVfx.transform.localScale = new Vector3(_baseScale.x, _baseScale.y, lenLS); 
    }
    
    private IDamageable FindClosest(float range)
    {
        Collider[] hits = Physics.OverlapSphere(_playerContext.transform.position, range);
        IDamageable best = null;
        float bestSqr = float.MaxValue;

        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out IDamageable d)) continue;
            float sqr = (h.transform.position - _playerContext.transform.position).sqrMagnitude;
            if (sqr < bestSqr) { best = d; bestSqr = sqr; }
        }
        return best;
    }
    
    private void Stop()
    {
        if (!_running) return;
        _running = false;

        _castVfx.Stop();
        _beamVfx.Stop();
        _hitVfx.Stop();
        Destroy(gameObject, 0.25f);
    }

    private void OnDisable() => Stop();
}
