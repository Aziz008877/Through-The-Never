using UnityEngine;
using Zenject;
public class PlayerContext : ActorContext
{
    [SerializeField] private SkillSelectionSaver _selectionSaver;
    [Inject] private PlayerMove _playerMove;
    [Inject] private PlayerInput _playerInput;
    [Inject] private PlayerHP _playerHp;
    [Inject] private PlayerAnimator _playerAnimator;
    [Inject] private PlayerSkillManager _playerSkillManager;
    [Inject] private PlayerEnemyHandler _playerEnemyHandler;
    [Inject] private PlayerState _playerState;
    public SkillSelectionSaver SkillSelectionSaver => _selectionSaver;
    public PlayerInput PlayerInput => _playerInput;
    public override IActorHp Hp => _playerHp;
    public override IActorMove Move => _playerMove;
    public override IActorAnim Animator => _playerAnimator;
    public override IActorState State => _playerState;
    public override IEnemyHandler EnemyHandler => _playerEnemyHandler;
    public override ISkillManager SkillManager => _playerSkillManager;
}
