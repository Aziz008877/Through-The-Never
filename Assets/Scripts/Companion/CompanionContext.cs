public class CompanionContext : ActorContext
{
    CompanionHp        _hp;
    CompanionMove      _move;
    CompanionAnimation _anim;
    CompanionState     _state;

    void Awake()
    {
        _hp   = GetComponent<CompanionHp>();
        _move = GetComponent<CompanionMove>();
        _anim = GetComponent<CompanionAnimation>();
        _state= GetComponent<CompanionState>();
    }

    public override IActorHp    Hp       => _hp;
    public override IActorMove  Move     => _move;
    public override IActorAnim  Animator => _anim;
    public override IActorState State    => _state;
}