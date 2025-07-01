using System;
using UnityEngine;
using Zenject;

public class FirebeamSkill : BaseSkill
{
    [Header("Firebeam Components")]
    [SerializeField] private ParticleSystem _castParticles;
    [SerializeField] private ParticleSystem _firebeamParticles;
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private Transform _beamStartPoint;
    [Inject] private PlayerInput _playerInput;
    [Inject] private PlayerAnimator _playerAnimator;

    private bool _isCasting = false;
    public bool IsCasting => _isCasting;
    private float _castTimer = 0f;

    private void Awake()
    {
        _playerInput.OnDefensiveSkillPressed += FireBeamState;
    }

    private void OnDestroy()
    {
        _playerInput.OnDefensiveSkillPressed -= FireBeamState;
    }

    private void FireBeamState()
    {
        if (!_isCasting && IsReady)
        {
            StartBeam();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_isCasting)
        {
            _castTimer += Time.deltaTime;
            UpdateBeam();

            if (_castTimer >= _duration)
            {
                StopBeam();
            }
        }
    }

    private void StartBeam()
    {
        _isCasting = true;
        _castTimer = 0f;
        _cooldownTimer = _coolDown;

        _playerAnimator.CastBeam(true);

        _castParticles.Play();
        _firebeamParticles.Play();
    }

    private void StopBeam()
    {
        _isCasting = false;
        _playerAnimator.CastBeam(false);
        _castParticles.Stop();
        _firebeamParticles.Stop();
        _hitParticles.Stop();
    }

    private void UpdateBeam()
    {
        Vector3 direction = _beamStartPoint.forward;
        Vector3 startPoint = _beamStartPoint.position;
        Vector3 endPoint = startPoint + direction * _range;

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, _range))
        {
            endPoint = hit.point;

            _hitParticles.transform.position = hit.point;
            if (!_hitParticles.isPlaying)
                _hitParticles.Play();

            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.ReceiveDamage(_damage * Time.deltaTime, _skillDamageType);
            }
        }
        else
        {
            if (_hitParticles.isPlaying)
                _hitParticles.Stop();
        }

        AdjustBeamLength(Vector3.Distance(startPoint, endPoint));
    }

    private void AdjustBeamLength(float length)
    {
        _firebeamParticles.transform.localScale = new Vector3(
            _firebeamParticles.transform.localScale.x,
            _firebeamParticles.transform.localScale.y,
            length
        );
    }
}
