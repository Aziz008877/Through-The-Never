using System.Collections;
using UnityEngine;

public class SplinterStrikeSkill : ActiveSkillBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private IceShard _shardPrefab;
    [SerializeField] private DeepfrostWaterProjectile _deepfrostWaterProjectile;
    [SerializeField] private float _lifeTime = 3f;

    [Header("Burst")]
    [SerializeField] private int _shotsPerBurst = 10;
    [SerializeField] private float _minDelayBetween = 0.0f;
    [SerializeField] private float _maxDelayBetween = 0.06f;
    [SerializeField, Range(0f,1f)] private float _delayChance = 0.5f;

    [Header("Spread")]
    [SerializeField] private float _spreadAngleDeg = 2.2f;
    [SerializeField] private float _posJitter = 0.05f;
    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        PlayerCtx.Move.RotateTowardsMouse();

        StartCoroutine(BurstRoutine());
        StartCooldown();
    }

    private IEnumerator BurstRoutine()
    {
        Vector3 mainDir = Context.CastPivot.forward;
        bool empowered = Context.DeepfrostWaterMode;

        if (empowered)
        {
            var pos = Context.CastPivot.position;
            var rot = Quaternion.LookRotation(mainDir, Vector3.up);

            var beam = Instantiate(_deepfrostWaterProjectile, pos, rot);
            beam.Init(Damage, _lifeTime, SkillDamageType.Basic, Context);

            Vector3 firePoint = Context.CastPivot.position + mainDir * 5f;
            PlayerBasicAttackEvents.Fire(firePoint);

            StartCooldown();
            yield break;
        }

        Vector3 right = Context.transform.right;
        Vector3 up = Vector3.up;

        for (int i = 0; i < _shotsPerBurst; i++)
        {
            float yaw = Random.Range(-_spreadAngleDeg, _spreadAngleDeg);
            float pitch = Random.Range(-_spreadAngleDeg * 0.5f, _spreadAngleDeg * 0.5f);
            Vector3 dir = Quaternion.AngleAxis(yaw, up) * (Quaternion.AngleAxis(pitch, right) * mainDir);

            Vector3 offset = right * Random.Range(-_posJitter, _posJitter);
            Vector3 pos = Context.CastPivot.position + offset;

            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            var shard = Instantiate(_shardPrefab, pos, rot);
            shard.Init(Damage, _lifeTime, SkillDamageType.Basic, Context);

            if (i == 0)
            {
                Vector3 firePoint = Context.CastPivot.position + mainDir * 5f;
                PlayerBasicAttackEvents.Fire(firePoint);
            }

            if (_delayChance > 0f && Random.value < _delayChance)
            {
                float d = (_maxDelayBetween <= _minDelayBetween) ? _minDelayBetween : Random.Range(_minDelayBetween, _maxDelayBetween);
                if (d > 0f) yield return new WaitForSeconds(d);
            }
        }

        StartCooldown();
    }

}
