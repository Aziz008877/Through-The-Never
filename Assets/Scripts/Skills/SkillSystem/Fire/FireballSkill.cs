using UnityEngine;
public class FireballSkill : ActiveSkillBehaviour
{
    [Header("VFX & Spawn")]
    [SerializeField] private Fireball _fireballPrefab;
    [SerializeField] private float _projectileLifeTime = 3f;
    public override void TryCast()
    {
        if (!IsReady) return;

        var projectile = Instantiate(_fireballPrefab,
            PlayerContext.PlayerCastPosition.position,
            PlayerContext.PlayerCastPosition.rotation);

        projectile.Init(Definition.Damage, _projectileLifeTime, SkillDamageType.Basic, PlayerContext);

        StartCooldown();
    }
}