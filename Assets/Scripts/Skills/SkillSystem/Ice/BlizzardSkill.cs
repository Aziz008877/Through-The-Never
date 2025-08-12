using System;
using UnityEngine;

public class BlizzardSkill : ActiveSkillBehaviour
{
    [SerializeField] private BlizzardAura _auraPrefab;
    [SerializeField] private float _tickRate = 0.25f;
    [SerializeField, Range(0f,1f)] private float _maxSlow = 0.4f;
    [SerializeField] private int _frostMaxStacks = 5;

    private BlizzardAura _instance;
    private PlayerSkillManager _skillManager;

    public override void Inject(SkillDefinition definition, ActorContext context)
    {
        base.Inject(definition, context);
        if (context is PlayerContext pc) _skillManager = pc.GetComponent<PlayerSkillManager>();
    }

    public override void TryCast()
    {
        if (!IsReady || _instance != null) return;
        base.TryCast();

        if (_skillManager != null) _skillManager.SetBasicLocked(true);

        _instance = Instantiate(_auraPrefab, Context.transform.position, Quaternion.identity);
        _instance.Bind(Context.transform, Radius, Duration, _tickRate, Damage, _frostMaxStacks, _maxSlow, Context);
        _instance.OnFinished += OnAuraFinished;
    }

    private void OnAuraFinished()
    {
        if (_instance != null) _instance.OnFinished -= OnAuraFinished;
        _instance = null;
        if (_skillManager != null) _skillManager.SetBasicLocked(false);
        StartCooldown();
    }

    private void OnDisable()
    {
        if (_instance != null) Destroy(_instance.gameObject);
        if (_skillManager != null) _skillManager.SetBasicLocked(false);
    }
}