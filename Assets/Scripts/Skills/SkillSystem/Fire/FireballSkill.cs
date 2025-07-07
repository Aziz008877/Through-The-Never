using UnityEngine;

public class FireballSkill : ActiveSkillBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Fireball _normalPrefab;
    [SerializeField] private BigFireball _bigPrefab;
    [SerializeField] private float _lifeTime = 3f;

    [Header("Extra projectiles")]
    [SerializeField] private float _sideOffset = 0.4f;
    [SerializeField] private float _angleStep = 7f;

    private int  _extraProjectiles;
    private bool _smallExplosionEnabled;
    private bool _homingEnabled;

    public void SetExtraProjectiles(int count)   => _extraProjectiles = Mathf.Max(0, count);
    public void SetSmallExplosion   (bool state) => _smallExplosionEnabled = state;
    public void SetHomingProjectiles(bool state) => _homingEnabled = state;
    public override void TryCast()
    {
        if (!IsReady) return;

        bool empowered = PlayerContext.SolarFlareCharge;
        Debug.Log(empowered);
        Fireball prefabCtr = empowered ? _bigPrefab : _normalPrefab;

        Shoot(prefabCtr, Vector3.zero, PlayerContext.PlayerCastPosition.forward);

        if (_extraProjectiles > 0)
        {
            Vector3 baseDir = PlayerContext.PlayerCastPosition.forward;
            Vector3 right   = PlayerContext.transform.right;

            for (int i = 1; i <= _extraProjectiles; i++)
            {
                Vector3 dirR = Quaternion.AngleAxis(+_angleStep * i, Vector3.up) * baseDir;
                Vector3 dirL = Quaternion.AngleAxis(-_angleStep * i, Vector3.up) * baseDir;

                Shoot(_normalPrefab,  right * _sideOffset * i, dirR);
                Shoot(_normalPrefab, -right * _sideOffset * i, dirL);
            }
        }

        if (empowered) PlayerContext.SolarFlareCharge = false;
        StartCooldown();
    }

    private void Shoot(Fireball prefab, Vector3 spawnOffset, Vector3 shootDir)
    {
        Vector3 pos = PlayerContext.PlayerCastPosition.position + spawnOffset;
        Quaternion rot = Quaternion.LookRotation(shootDir, Vector3.up);

        Fireball proj = Instantiate(prefab, pos, rot);
        proj.EnableSmallExplosion(_smallExplosionEnabled);
        proj.SetHoming(_homingEnabled);
        proj.Init(Damage, _lifeTime, SkillDamageType.Basic, PlayerContext);
    }
}
