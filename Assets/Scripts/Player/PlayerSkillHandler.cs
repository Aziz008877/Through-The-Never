using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerSkillHandler : MonoBehaviour
{
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private ParticleSystem _aoeParticles;
    [SerializeField] private Transform _spawnParent;
    [Inject] private PlayerInput _playerInput;
    [Inject] private PlayerAnimator _playerAnimator;
    [Inject] private CameraShake _cameraShake;
    private void Awake()
    {
        _playerInput.OnPlayerPressedBasic += PerformBasicSkill;
        _playerInput.OnPlayerAOE += AOESkill;
    }

    private void AOESkill()
    {
        _aoeParticles.Play();
    }

    private async void PerformBasicSkill()
    {
        _playerAnimator.CastBasics();
        await Task.Delay(200);
        GameObject fireball = Instantiate(_fireBall, _spawnParent.position, _spawnParent.rotation);
        _cameraShake.Shake();
        fireball.GetComponent<Fireball>().Init();
    }

    private void OnDestroy()
    {
        _playerInput.OnPlayerPressedBasic -= PerformBasicSkill;
        _playerInput.OnPlayerAOE -= AOESkill;
    }
}
