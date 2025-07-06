using UnityEngine;
public class AspectOfSolSkill : ActiveSkillBehaviour
{
    [SerializeField] private AspectOfSolOrb _orbPrefab;
    [SerializeField] private float _projectileSpeed = 14f;
    [SerializeField] private float _fireRate = .8f;
    public override void TryCast()
    {
        if (!IsReady) return;
        var orb = Instantiate(_orbPrefab, PlayerContext.transform.position, Quaternion.identity);
        orb.Init(Damage, _projectileSpeed, _fireRate, Definition.Raduis, Definition.Duration, PlayerContext);
        StartCooldown();
    }
}