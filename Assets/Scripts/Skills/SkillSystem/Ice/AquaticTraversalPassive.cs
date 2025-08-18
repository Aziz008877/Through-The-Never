using UnityEngine;

public sealed class AquaticTraversalPassive : PassiveSkillBehaviour
{
    [Header("Aquatic Traversal")]
    [SerializeField, Range(0f, 1f)] private float _evasionChance = 0.35f;
    [SerializeField] private float _buffDuration = 2.0f;
    [SerializeField] private ParticleSystem _evadeVfx;

    private PlayerDashSkill _dash;
    [SerializeField] private bool  _buffActive;
    private float _timeLeft;

    public override void EnablePassive()
    {
        Attach(Context.SkillManager.GetActive(SkillSlot.Dash));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;

        Context.Hp.OnIncomingDamage += OnIncomingDamage;
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();

        Context.Hp.OnIncomingDamage -= OnIncomingDamage;

        StopBuff();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashEnded += OnDashEnded;
        }
    }

    private void Detach()
    {
        if (_dash == null) return;
        _dash.OnDashEnded -= OnDashEnded;
        _dash = null;
    }

    private void OnDashEnded(Vector3 endPos)
    {
        _buffActive = true;
        _timeLeft = _buffDuration;

        if (_evadeVfx) { _evadeVfx.transform.position = Context.transform.position; _evadeVfx.Play(); }
    }

    private void Update()
    {
        if (!_buffActive) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f) StopBuff();
    }

    private void StopBuff()
    {
        if (!_buffActive) return;
        _buffActive = false;
        if (_evadeVfx) _evadeVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnIncomingDamage(ref float damage, IDamageable source)
    {
        if (!_buffActive || damage <= 0f) return;
        if (Random.value < _evasionChance) damage = 0f;
    }
}
