using UnityEngine;

public class AspectOfSolSkill : ActiveSkillBehaviour
{
    [SerializeField] private AspectOfSolOrb _orbPrefab;
    [SerializeField] private float _projectileSpeed = 14f;
    [SerializeField] private float _fireRate = 0.8f;

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        float damage = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Damage), Definition.Damage);
        float duration = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Duration), Definition.Duration);
        float radius = Context.SkillModifierHub.Apply(new SkillKey(Definition.Slot, SkillStat.Radius), Definition.Raduis);

        var orb = Instantiate(_orbPrefab, Context.CastPivot.position, Quaternion.identity);
        orb.Init(damage, _projectileSpeed, _fireRate, radius, duration, Context);
        StartCooldown();
    }
}