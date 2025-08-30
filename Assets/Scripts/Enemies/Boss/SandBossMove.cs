public class SandBossMove : BaseEnemyMove
{
    protected override void Start()
    {
        base.Start();
        _behaviour = MoveBehaviour.ChaseAlways;
        _chaseRange = 999f;
    }
}
