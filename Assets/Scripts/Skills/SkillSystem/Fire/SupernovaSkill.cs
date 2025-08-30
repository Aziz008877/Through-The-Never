using UnityEngine;

public class SupernovaSkill : ActiveSkillBehaviour
{
    [SerializeField] private ParticleSystem _chargeVfx;
    [SerializeField] private ParticleSystem _blastVfx;
    [SerializeField] private float _maxChargeTime = 3f;
    [SerializeField] private float _minRadius = 3f;
    [SerializeField] private float _maxRadius = 8f;
    [SerializeField] private float _minDamage = 15f;
    [SerializeField] private float _maxDamage = 60f;

    private bool _charging;
    private float _timer;

    public override void Inject(SkillDefinition definition, ActorContext context)
    {
        base.Inject(definition, context);
        
        if (context is PlayerContext pc)
        {
            pc.PlayerInput.OnSpecialSkillPressed  += TryCast;
            pc.PlayerInput.OnSpecialSkillReleased += Release;
        }
    }

    public override void TryCast()
    {
        if (!IsReady || _charging) return;
        base.TryCast();
        _charging = true;
        _timer = 0f;
        _chargeVfx.Play();
    }

    protected override void Update()
    {
        base.Update();
        if (!_charging) return;
        _timer += Time.deltaTime;
        if (_timer >= _maxChargeTime) Release();
    }

    private void Release()
    {
        if (!_charging) return;
        _charging = false;
        _chargeVfx.Stop();

        float t = Mathf.Clamp01(_timer / _maxChargeTime);

        float radius = Mathf.Lerp(_minRadius, _maxRadius, t);
        radius = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), radius);

        float damage = Mathf.Lerp(_minDamage, _maxDamage, t);

        var hits = Physics.OverlapSphere(Context.transform.position, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].TryGetComponent(out IDamageable d)) continue;
            var ctx = BuildDamage(damage, SkillDamageType.Basic,
                hitPoint: hits[i].transform.position,
                hitNormal: Vector3.up,
                sourceGO: gameObject);
            ctx.Target = d;

            Context.ApplyDamageContextModifiers(ref ctx);

            d.ReceiveDamage(ctx);
        }

        if (_blastVfx != null)
        {
            _blastVfx.transform.position = Context.transform.position;
            _blastVfx.transform.localScale = Vector3.one * radius * 0.5f;
            _blastVfx.Play();
        }

        StartCooldown();
    }


    private void OnDisable()
    {
        if (PlayerCtx != null)
        {
            PlayerCtx.PlayerInput.OnSpecialSkillPressed  -= TryCast;
            PlayerCtx.PlayerInput.OnSpecialSkillReleased -= Release;
        }
    }
}
