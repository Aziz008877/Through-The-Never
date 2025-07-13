using UnityEngine;

public class FirebeamSkill : ActiveSkillBehaviour
{
    [Header("Prefabs / timing")]
    [SerializeField] private FirebeamBeam _beamPrefab;
    [SerializeField] private float _tickRate = .25f;
    [SerializeField] private float _baseDps = 4f;
    [SerializeField] private float _maxDps = 16f;

    public override void TryCast()
    {
        if (!IsReady) return;
        
        float duration = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Duration), Definition.Duration);
        float range = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Range),    Definition.Range);
        
        var beam = Instantiate(_beamPrefab, PlayerContext.PlayerPosition.position, Quaternion.identity);
        beam.Init(PlayerContext, duration, range, _tickRate, _baseDps, _maxDps);

        StartCooldown();
    }
}