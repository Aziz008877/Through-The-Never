using UnityEngine;

public class RangedMobMove : BaseEnemyMove
{
    [SerializeField] private float _tier1Speed = 2.2f;
    [SerializeField] private float _tier2Speed = 3.0f;
    [SerializeField] private float _tier3Speed = 3.8f;

    private RangedMobAttack _atk;

    protected override void Start()
    {
        _atk = GetComponent<RangedMobAttack>();
        base.Start();
        _behaviour = MoveBehaviour.ChaseAlways;

        float baseSpd = _tier1Speed;
        if (_atk.Tier == RangedMobTier.Tier2_Orange) baseSpd = _tier2Speed;
        else if (_atk.Tier == RangedMobTier.Tier3_Purple) baseSpd = _tier3Speed;

        _agent.speed = baseSpd;
    }
}
