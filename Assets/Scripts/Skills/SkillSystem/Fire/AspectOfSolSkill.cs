using UnityEngine;

public class AspectOfSolSkill : ActiveSkillBehaviour
{
    [SerializeField] private AspectOfSolOrb _orbPrefab;
    [SerializeField] private float _projectileSpeed = 14f;
    [SerializeField] private float _fireRate = 0.8f;

    public override void TryCast()
    {
        if (!IsReady) return;
        
        float damage = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), Definition.Damage);
        float duration = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Duration), Definition.Duration);
        float radius = PlayerContext.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), Definition.Raduis);

        var orb = Instantiate(_orbPrefab, PlayerContext.PlayerCastPosition.position, Quaternion.identity);
        orb.Init(damage, _projectileSpeed, _fireRate, radius, duration, PlayerContext);
        StartCooldown();
    }
}