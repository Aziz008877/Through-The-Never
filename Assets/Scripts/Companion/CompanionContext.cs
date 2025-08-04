using System;
using UnityEngine;

public class CompanionContext : ActorContext
{
    private CompanionHp _hp;
    private CompanionMove _move;
    private CompanionAnimation  _anim;
    private CompanionState _state;
    public override IActorHp Hp => _hp;
    public override IActorMove Move => _move;
    public override IActorAnim Animator => _anim;
    public override IActorState State => _state;

    private void Start()
    {
        _hp = GetComponent<CompanionHp>();
        _move = GetComponent<CompanionMove>();
        _anim = GetComponent<CompanionAnimation>();
        _state = GetComponent<CompanionState>();
    }
}