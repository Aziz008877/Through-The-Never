using UnityEngine;
public class AspectOfSolSkill : ActiveSkillBehaviour
{
    [SerializeField] private AspectOfSolOrb _orbPrefab;
    [SerializeField] private float _orbLifetime = 15f;
    [SerializeField] private float _projectileSpeed = 14f;
    [SerializeField] private float _fireRate = .8f;
    [SerializeField] private float _detectRadius = 12f;
    public override void TryCast()
    {
        if (!IsReady) return;
        var orb = Instantiate(_orbPrefab, PlayerContext.transform.position, Quaternion.identity);
        orb.Init(Definition.Damage, _projectileSpeed, _fireRate, _detectRadius, _orbLifetime, PlayerContext);
        StartCooldown();
    }
}