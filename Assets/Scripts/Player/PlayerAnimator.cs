using System;
using UnityEngine;
using Zenject;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private Fountain _fountain;
    [Inject] private PlayerState _playerState;
    [Inject] private PlayerDash _playerDash;
    [Inject] private PlayerMove _playerMove;
    private readonly int _isMoving = Animator.StringToHash("IsMoving");
    private readonly int _isRunning = Animator.StringToHash("IsRunning");
    private readonly int _isCrouching = Animator.StringToHash("IsCrouching");
    private readonly int _moveX = Animator.StringToHash("X");
    private readonly int _moveY = Animator.StringToHash("Y");
    private readonly int _basicAttack = Animator.StringToHash("BasicAttack");
    private readonly int _isShield = Animator.StringToHash("IsShield");
    private readonly int _isSitting = Animator.StringToHash("IsSitting");
    private readonly int _dash = Animator.StringToHash("Dash");

    private void Awake()
    {
        if (_fountain != null)
        {
            _fountain.OnPlayerHealing += PlayerSitState;
        }
        
        _playerDash.OnPlayerDash += Dash;
        _playerMove.OnPlayerMove += ReceivePlayerMoveState;
        _playerMove.OnPlayerSprint += Sprint;
        _playerMove.OnPlayerCrouch += Crouch;
    }

    private void PlayerSitState(bool sitState)
    {
        _playerAnimator.SetBool(_isSitting, sitState);
        _playerState.ChangePlayerState(!sitState);

        if (sitState)
        {
            _playerAnimator.SetBool(_isMoving, false);
        }
    }

    private void Dash()
    {
        _playerAnimator.SetTrigger(_dash);
    }

    private void Crouch(bool crouchState)
    {
        _playerAnimator.SetBool(_isCrouching, crouchState);
    }
    

    public void CastBasics()
    {
        _playerAnimator.SetTrigger(_basicAttack);
    }

    public void CastShield(bool state)
    {
        _playerAnimator.SetBool(_isShield, state);
    }

    private void Sprint(bool sprintState)
    {
        _playerAnimator.SetBool(_isRunning, sprintState);
    }

    private void ReceivePlayerMoveState(Vector3 direction)
    {
        _playerAnimator.SetBool(_isMoving, direction != Vector3.zero);

        if (direction.magnitude > 0.01f)
        {
            Vector3 localDir = transform.InverseTransformDirection(direction.normalized);
            _playerAnimator.SetFloat(_moveX, localDir.x);
            _playerAnimator.SetFloat(_moveY, localDir.z);
        }
        else
        {
            _playerAnimator.SetFloat(_moveX, 0f);
            _playerAnimator.SetFloat(_moveY, 0f);
        }
    }
    
    private void OnDestroy()
    {
        if (_fountain != null)
        {
            _fountain.OnPlayerHealing -= PlayerSitState;
        }
        
        _playerMove.OnPlayerMove -= ReceivePlayerMoveState;
        _playerDash.OnPlayerDash -= Dash;
    }
}
