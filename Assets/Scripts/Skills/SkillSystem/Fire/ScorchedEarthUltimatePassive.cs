using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public sealed class ScorchedEarthUltimatePassive : PassiveSkillBehaviour, ISkillModifier
{
    [Header("Aura base")]
    [SerializeField] private float _baseRadius = 4f;
    [SerializeField] private float _baseDps = 10f;
    [SerializeField] private ParticleSystem _auraVfx;

    [Header("Stack buffs")]
    [SerializeField] private float _radiusPerStackPct = .20f;
    [SerializeField] private float _dpsPerStackPct = .30f;
    [SerializeField] private float _spellDmgPerStackPct = .15f;
    [SerializeField] private float _spellRadPerStackPct = .10f;
    [SerializeField] private int _maxStacks = 3;
    [SerializeField] private float _stackDuration = 3f;
    [SerializeField] private float _tickRate = 1f;
    
    private int _stacks;
    private float _stackTimer;
    private Coroutine _tickRoutine;
    readonly List<ActiveSkillBehaviour> _attachedSkills = new();
    public override void EnablePassive()
    {
        AttachToExistingSkills();
        PlayerContext.PlayerSkillManager.ActiveRegistered += OnSkillRegistered;
        _tickRoutine = StartCoroutine(CoAuraTick());
        if (_auraVfx != null) _auraVfx.Play(true);
        PlayerContext.SkillModifierHub.Register(this);
    }

    public override void DisablePassive()
    {
        DetachFromSkills();
        PlayerContext.PlayerSkillManager.ActiveRegistered -= OnSkillRegistered;

        if (_tickRoutine != null) StopCoroutine(_tickRoutine);
        if (_auraVfx != null) _auraVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        PlayerContext.SkillModifierHub.Unregister(this);
    }

    private void AttachToExistingSkills()
    {
        foreach (var kv in PlayerContext.PlayerSkillManager.Actives)
            Attach(kv.Value);
    }

    private void OnSkillRegistered(SkillSlot slot, ActiveSkillBehaviour beh) => Attach(beh);

    private void Attach(ActiveSkillBehaviour skill)
    {
        if (skill == null || _attachedSkills.Contains(skill)) return;
        skill.OnCooldownStarted += _ => AddStack();
        _attachedSkills.Add(skill);
    }

    private void DetachFromSkills()
    {
        foreach (ActiveSkillBehaviour s in _attachedSkills)
            if (s != null) s.OnCooldownStarted -= _ => AddStack();
        _attachedSkills.Clear();
    }

    private void AddStack()
    {
        _stacks = Mathf.Min(_stacks + 1, _maxStacks);
        _stackTimer = _stackDuration;
    }

    private void Update()
    {
        if (_stacks <= 0) return;

        _stackTimer -= Time.deltaTime;
        if (_stackTimer <= 0f)
        {
            _stacks--;
            _stackTimer = _stacks > 0 ? _stackDuration : 0f;
        }
    }

    private IEnumerator CoAuraTick()
    {
        var wait = new WaitForSeconds(_tickRate);

        while (true)
        {
            if (_stacks >= 0) DoAuraDamage();
            yield return wait;
        }
    }

    private void DoAuraDamage()
    {
        float radius = _baseRadius * (1f + _radiusPerStackPct * _stacks);
        float dps = _baseDps * (1f + _dpsPerStackPct * _stacks);
        float tickDmg = dps * _tickRate;

        if (_auraVfx != null)
            _auraVfx.transform.localScale = Vector3.one * radius * 0.5f;

        Collider[] hits = Physics.OverlapSphere(PlayerContext.transform.position, radius);
        foreach (Collider col in hits)
        {
            if (!col.TryGetComponent(out IDamageable enemy)) continue;

            SkillDamageType type = SkillDamageType.Basic;
            float dmg = tickDmg;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
            enemy.ReceiveDamage(dmg, type);

            PlayerContext.FireOnDamageDealt(enemy, dmg, type);
        }
    }

    public float Evaluate(SkillKey key, float currentValue)
    {
        if (_stacks == 0) return currentValue;

        if (key.Stat == SkillStat.Damage)
            return currentValue * (1f + _spellDmgPerStackPct * _stacks);

        if (key.Stat == SkillStat.Radius)
            return currentValue * (1f + _spellRadPerStackPct * _stacks);

        return currentValue;
    }
}
