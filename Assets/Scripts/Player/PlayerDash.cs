using System;
using UnityEngine;
using DG.Tweening;
using Zenject;
using Random = UnityEngine.Random;

public class PlayerDash : BaseSkill
{
    [Header("Dash")]
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private Transform _player;
    [SerializeField] private AudioSource _dashSound;
    [Inject] private PlayerMove _playerMove;
    [Inject] private PlayerInput _playerInput;
    [Inject] private FireAOESkill _fireAoeSkillSkill;
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
        if (!IsReady) return;
        base.PerformSkill();
        
        if (moveDir == Vector3.zero)
            moveDir = _player.forward;
        
        _dashTween?.Kill();
        _dashSound.pitch = Random.Range(0.9f, 1.5f);
        _dashSound.PlayOneShot(_dashSound.clip);
        Vector3 dashTarget = _player.position + moveDir.normalized * _dashDistance;
        OnPlayerDash?.Invoke();
        _dashParticles.Play();

        _dashTween = _player.DOMove(dashTarget, _dashDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(delegate
            {
                _fireAoeSkillSkill.PerformSkill(_playerMove.gameObject);
            });
    }
}