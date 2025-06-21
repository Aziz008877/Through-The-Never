using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    [Inject] private PlayerState _playerState;
    private Vector2 _moveInput;
    public Action<Vector2> OnMovePressed;
    public Action<bool> OnSprintPressed;
    public Action<bool> OnCrouchPressed;
    public Action<bool> OnShieldPressed;
    public Action OnPlayerJump;
    public Action OnPlayerPressedBasic;
    public Action OnPlayerAOE;
    private void Update()
    {
        if (_playerState.CurrentPlayerState == CurrentPlayerState.CanControl)
        {
            Move();
            BaseSkill();
            Shield();
            AOEDamage();
        }
    }

    private void Move()
    {
        float horizontalValue = Input.GetAxis("Horizontal");
        float verticalValue = Input.GetAxis("Vertical");

        _moveInput = new Vector2(horizontalValue, verticalValue);
        OnMovePressed?.Invoke(_moveInput);
    }

    private void Shield()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnShieldPressed?.Invoke(true);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            OnShieldPressed?.Invoke(false);
        }
    }

    private void BaseSkill()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnPlayerPressedBasic?.Invoke();
        }
    }

    private void AOEDamage()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnPlayerAOE?.Invoke();
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnSprintPressed?.Invoke(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnSprintPressed?.Invoke(false);
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnCrouchPressed?.Invoke(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            OnCrouchPressed?.Invoke(false);
        }
    }
}
