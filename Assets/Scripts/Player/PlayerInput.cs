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
    public Action OnDashPressed;
    private void Update()
    {
        if (_playerState.CurrentPlayerState == CurrentPlayerState.CanControl)
        {
            Move();
            BaseSkill();
            DefensiveSkill();
            SpecialSkill();
            Dash();
        }
    }

    private void Move()
    {
        float horizontalValue = Input.GetAxis("Horizontal");
        float verticalValue = Input.GetAxis("Vertical");

        _moveInput = new Vector2(horizontalValue, verticalValue);
        OnMovePressed?.Invoke(_moveInput);
    }

    private void BaseSkill()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            OnBasicSkillPressed?.Invoke();
        }
    }

    private void DefensiveSkill()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnDefensiveSkillPressed?.Invoke();
        }
    }

    private void SpecialSkill()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnSpecialSkillPressed?.Invoke();
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDashPressed?.Invoke();
        }
    }
}
