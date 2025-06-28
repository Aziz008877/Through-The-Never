/*
using System;
using UnityEngine;
using Zenject;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private GameObject _shield;
    [Inject] private PlayerInput _playerInput;
    [Inject] private PlayerAnimator _playerAnimator;
    private void Awake()
    {
        _playerInput.OnFireBeamPressed += ReceiveShieldState;
    }

    private void ReceiveShieldState(bool state)
    {
        _shield.SetActive(state);
        _playerAnimator.CastBeam(state);
    }

    private void OnDestroy()
    {
        _playerInput.OnFireBeamPressed -= ReceiveShieldState;
    }
}
*/
