using System;
using UnityEngine;
using DG.Tweening;
using Zenject;

public class PlayerDash : BaseSkill
{
    [Header("Dash")]
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private Transform _player;
    [Inject] private PlayerMove _playerMove;
    [Inject] private PlayerInput _playerInput;
    [Inject] private FireAOESkill _fireAoeSkillSkill;
    private bool _canDash = true;
    private Tween _dashTween;
    public Action OnPlayerDash;
    private void Awake()
    {
        _playerInput.OnPlayerDash += HandleDash;
    }

    private void HandleDash()
    {
        TryDash(_playerMove.LastMoveDirection);
    }

    private void TryDash(Vector3 moveDir)
    {
        if (!_canDash) return;

        if (moveDir == Vector3.zero)
            moveDir = _player.forward;
        
        _canDash = false;
        _dashTween?.Kill();

        Vector3 dashTarget = _player.position + moveDir.normalized * _dashDistance;
        OnPlayerDash?.Invoke();
        _dashParticles.Play();

        _dashTween = _player.DOMove(dashTarget, _dashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(delegate
            {
                _fireAoeSkillSkill.PerformSkill(_playerMove.gameObject);
                Invoke(nameof(ResetDash), _dashCooldown);
            });
    }
    
    private void ResetDash()
    {
        _canDash = true;
    }
}