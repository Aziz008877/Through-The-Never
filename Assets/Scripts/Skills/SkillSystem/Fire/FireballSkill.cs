using UnityEngine;

public class FireballSkill : ActiveSkillBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Fireball _normalPrefab;
    [SerializeField] private BigFireball _bigPrefab;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _sideOffset, _angleStep;
    private int _extraProjectiles;
    private bool _smallExplosionEnabled = false; 
    public  void SetExtraProjectiles(int count) => _extraProjectiles = Mathf.Max(0, count);
    public override void TryCast()
    {
        if (!IsReady) return;

        bool empowered = PlayerContext.SolarFlareCharge;
        Fireball prefabCenter = empowered ? _bigPrefab : _normalPrefab;
        
        Shoot(prefabCenter, Vector3.zero, PlayerContext.PlayerCastPosition.forward);

        if (_extraProjectiles > 0)
        {
            Vector3 baseDir = PlayerContext.PlayerCastPosition.forward;
            Vector3 right = PlayerContext.transform.right;

            for (int i = 1; i <= _extraProjectiles; i++)
            {
                Vector3 dirRight = Quaternion.AngleAxis(+_angleStep * i, Vector3.up) * baseDir;
                Vector3 dirLeft  = Quaternion.AngleAxis(-_angleStep * i, Vector3.up) * baseDir;

                Shoot(_normalPrefab,  right * _sideOffset * i, dirRight);
                Shoot(_normalPrefab, -right * _sideOffset * i, dirLeft);
            }
        }

        if (empowered) PlayerContext.SolarFlareCharge = false;
        StartCooldown();
    }
    
    private void Shoot(Fireball prefab, Vector3 spawnOffset, Vector3 shootDirection)
    {
        Vector3 pos = PlayerContext.PlayerCastPosition.position + spawnOffset;
        Quaternion rot = Quaternion.LookRotation(shootDirection, Vector3.up);

        Fireball proj = Instantiate(prefab, pos, rot);
        proj.EnableSmallExplosion(_smallExplosionEnabled);
        proj.Init(Damage, _lifeTime, SkillDamageType.Basic, PlayerContext);
    }

    public void SetSmallExplosion(bool enable)
    {
        _smallExplosionEnabled = enable;
    }
}