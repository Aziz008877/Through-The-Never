using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    [Inject] private PlayerState _playerState;

    private Vector2 _moveInput;

    public Action<Vector2> OnMovePressed;
    public Action OnBasicSkillPressed;

    public Action OnDefensiveSkillPressed;
    public Action OnSpecialSkillPressed;
    public Action OnSpecialSkillReleased;
    public Action OnDashPressed;
    public Action ChangeInventoryState;

    private void Update()
    {
        if (_playerState.CurrentPlayerState != CurrentPlayerState.CanControl) return;

        Move();
        BaseSkill();
        DefensiveSkill();
        SpecialSkill();
        Dash();
        OpenInventory();
    }

    private void Move()
    {
        _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMovePressed?.Invoke(_moveInput);
    }

    private void BaseSkill()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            OnBasicSkillPressed?.Invoke();
    }

    private void DefensiveSkill()
    {
        if (Input.GetMouseButtonDown(1))
            OnDefensiveSkillPressed?.Invoke();
    }

    private void SpecialSkill()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnSpecialSkillPressed?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            OnSpecialSkillReleased?.Invoke();
        }
    }

    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeInventoryState?.Invoke();
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnDashPressed?.Invoke();
    }
}