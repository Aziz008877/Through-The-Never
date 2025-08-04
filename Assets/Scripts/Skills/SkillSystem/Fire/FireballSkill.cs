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

    public void SetExtraProjectiles (int count) => _extraProjectiles = Mathf.Max(0, count);
    public void SetSmallExplosion (bool state) => _smallExplosionEnabled = state;
    public void SetHomingProjectiles (bool state) => _homingEnabled = state;
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();
        
        if (PlayerCtx != null)
            PlayerCtx.Move.RotateTowardsMouse();

        bool empowered = Context.SolarFlareCharge;
        Fireball prefabCtr = empowered ? _bigPrefab : _normalPrefab;
        
        Vector3 mainDir = Context.CastPivot.forward;
        Shoot(prefabCtr, Vector3.zero, mainDir);
        
        Vector3 firePoint = Context.CastPivot.position + mainDir * 5;
        PlayerBasicAttackEvents.Fire(firePoint);
        
        if (_extraProjectiles > 0)
        {
            Vector3 right = Context.transform.right;
            for (int i = 1; i <= _extraProjectiles; i++)
            {
                Vector3 dirR = Quaternion.AngleAxis(+_angleStep * i, Vector3.up) * mainDir;
                Vector3 dirL = Quaternion.AngleAxis(-_angleStep * i, Vector3.up) * mainDir;

                Shoot(_normalPrefab,  right * _sideOffset * i, dirR);
                Shoot(_normalPrefab, -right * _sideOffset * i, dirL);
            }
        }

        if (empowered) Context.SolarFlareCharge = false;
        StartCooldown();
    }

    private void Shoot(Fireball prefab, Vector3 spawnOffset, Vector3 shootDir)
    {
        Vector3 pos = Context.CastPivot.position + spawnOffset;
        Quaternion rot = Quaternion.LookRotation(shootDir, Vector3.up);

        Fireball proj = Instantiate(prefab, pos, rot);
        proj.EnableSmallExplosion(_smallExplosionEnabled);
        proj.SetHoming(_homingEnabled);
        proj.Init(Damage, _lifeTime, SkillDamageType.Basic, Context);
    }
}
