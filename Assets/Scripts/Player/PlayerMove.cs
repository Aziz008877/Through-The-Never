using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _crouchSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCamera;

    private PlayerInput _playerInput;
    private float _speed;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isCrouching;

    public Action<Vector3> OnPlayerMove;
    public Action<bool> OnPlayerSprint;
    public Action<bool> OnPlayerCrouch;

    private void Start()
    {
        _speed = _moveSpeed;
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;

        _playerInput.OnMovePressed += ReceivePressedValue;
        _playerInput.OnSprintPressed += Sprint;
        _playerInput.OnCrouchPressed += Crouch;
    }

    private void Crouch(bool state)
    {
        _isCrouching = state;
        _speed = state ? _crouchSpeed : _moveSpeed;
        OnPlayerCrouch?.Invoke(state);
    }

    private void Sprint(bool state)
    {
        if (_isCrouching) return;

        _isSprinting = state;
        _speed = state ? _runSpeed : _moveSpeed;
        OnPlayerSprint?.Invoke(state);
    }

    private void ReceivePressedValue(Vector2 moveInput)
    {
        _isMoving = moveInput != Vector2.zero;

        RotateTowardsMouse();
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveDir = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * inputDirection;
        
        if (_isMoving)
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
        }
        
        OnPlayerMove?.Invoke(moveDir);
    }

    private void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 lookDirection = hitPoint - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
}
