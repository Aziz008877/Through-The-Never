using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerInput))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    private bool _canDash = true;
    private Tween _dashTween;
    private PlayerMove _playerMove;
    private PlayerAnimator _playerAnimator;
    private PlayerInput _playerInput;
    public Action OnPlayerDash;
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.OnPlayerDash += HandleDash;
        _playerMove = GetComponent<PlayerMove>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void HandleDash()
    {
        TryDash(_playerMove.LastMoveDirection);
    }

    private void TryDash(Vector3 moveDir)
    {
        if (!_canDash) return;

        if (moveDir == Vector3.zero)
            moveDir = transform.forward;
        
        _canDash = false;
        _dashTween?.Kill();

        Vector3 dashTarget = transform.position + moveDir.normalized * _dashDistance;
        OnPlayerDash?.Invoke();
        _dashTween = transform.DOMove(dashTarget, _dashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(delegate
            {
                Invoke(nameof(ResetDash), _dashCooldown);
            });
    }
    
    private void ResetDash()
    {
        _canDash = true;
    }
}