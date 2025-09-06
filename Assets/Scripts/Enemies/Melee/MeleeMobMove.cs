using UnityEngine;

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
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        _baseSpeed = _tier1Speed;
        switch (_attack != null ? _attack.Tier : MeleeMobTier.Tier1_Blue)
        {
            case MeleeMobTier.Tier2_Cyan:  _baseSpeed = _tier2Speed; break;
            case MeleeMobTier.Tier3_Green: _baseSpeed = _tier3Speed; break;
            case MeleeMobTier.Tier4_Red:   _baseSpeed = _tier4Speed; break;
        }
        _agent.speed = _baseSpeed;

        base.Start();
        _behaviour = MoveBehaviour.ChaseAlways;
    }

    protected override void Update()
    {
        base.Update();
        if (AgentReady)
            _agent.speed = _isMoving && _attack ? _baseSpeed * _attack.CurrentRoarSpeedMul : _baseSpeed;
    }
    
    public void RecalculateBaseSpeed()
    {
        if (!_attack) return;
        switch (_attack.Tier)
        {
            case MeleeMobTier.Tier1_Blue:  _baseSpeed = _tier1Speed; break;
            case MeleeMobTier.Tier2_Cyan:  _baseSpeed = _tier2Speed; break;
            case MeleeMobTier.Tier3_Green: _baseSpeed = _tier3Speed; break;
            case MeleeMobTier.Tier4_Red:   _baseSpeed = _tier4Speed; break;
        }
    }
}