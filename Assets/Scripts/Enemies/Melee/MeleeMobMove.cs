using UnityEngine;
using UnityEngine.AI;

public class MeleeMobMove : BaseEnemyMove
{
    [Header("Tier Base Speed")]
    [SerializeField] private float _tier1Speed = 2.2f;
    [SerializeField] private float _tier2Speed = 3.0f;
    [SerializeField] private float _tier3Speed = 3.0f;
    [SerializeField] private float _tier4Speed = 3.6f;

    private MeleeMobAttack _attack;

    protected override void Start()
    {
        _attack = GetComponent<MeleeMobAttack>();
        _agent = GetComponent<NavMeshAgent>();

        float baseSpeed = _tier1Speed;
        switch (_attack != null ? _attack.Tier : MeleeMobTier.Tier1_Blue)
        {
            case MeleeMobTier.Tier2_Cyan:  baseSpeed = _tier2Speed; break;
            case MeleeMobTier.Tier3_Green: baseSpeed = _tier3Speed; break;
            case MeleeMobTier.Tier4_Red:   baseSpeed = _tier4Speed; break;
        }
        _agent.speed = baseSpeed;

        base.Start();
        _behaviour = MoveBehaviour.ChaseAlways;
    }

    protected override void Update()
    {
        base.Update();
        if (AgentReady && _isMoving && _attack != null)
            _agent.speed *= _attack.CurrentRoarSpeedMul;
    }
}
