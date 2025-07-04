using UnityEngine;

public class FireballSkill : ActiveSkillBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Fireball _normalPrefab;
    [SerializeField] private BigFireball _bigPrefab;
    [SerializeField] private float _lifeTime = 3f;

    public override void TryCast()
    {
        if (!IsReady) return;

        bool empowered = PlayerContext.SolarFlareCharge;
        Fireball prefab = empowered ? _bigPrefab : _normalPrefab;

        var proj = Instantiate(prefab,
            PlayerContext.PlayerCastPosition.position,
            PlayerContext.PlayerCastPosition.rotation);

        proj.Init(Definition.Damage, _lifeTime, SkillDamageType.Basic, PlayerContext);

        if (empowered) PlayerContext.SolarFlareCharge = false;
        StartCooldown();
    }
}