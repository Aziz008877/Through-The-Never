using UnityEngine;
using Zenject;
public class PlayerAnimator : MonoBehaviour, IActorAnim
{
    [SerializeField] private Animator _playerAnimator;
    [Inject] private PlayerState _playerState;
    [Inject] private PlayerMove _playerMove;
    private readonly int _isMoving = Animator.StringToHash("IsMoving");
    private readonly int _moveX = Animator.StringToHash("X");
    private readonly int _moveY = Animator.StringToHash("Y");
    private readonly int _basicAttack = Animator.StringToHash("BasicAttack");
    private readonly int _isShield = Animator.StringToHash("IsShield");
    private readonly int _isSitting = Animator.StringToHash("IsSitting");
    private readonly int _dash = Animator.StringToHash("Dash");
    private void Awake()
    {
        _playerMove.OnPlayerMove += ReceivePlayerMoveState;
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

    public void Dash()
    {
        _playerAnimator.SetTrigger(_dash);
    }

    public void CastBasics()
    {
        _playerAnimator.SetTrigger(_basicAttack);
    }

    public void CastBeam(bool state)
    {
        _playerAnimator.SetBool(_isShield, state);
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

    public void DeactivatePlayerMove()
    {
        _playerAnimator.SetBool(_isMoving, false);
    }
    
    private void OnDestroy()
    {
        _playerMove.OnPlayerMove -= ReceivePlayerMoveState;
    }
}
