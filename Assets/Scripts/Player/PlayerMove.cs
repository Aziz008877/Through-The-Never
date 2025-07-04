using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCamera;
    public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;

    private PlayerInput _playerInput;
    private float _speed;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isCrouching;
    public Action<Vector3> OnPlayerMove;

    private float _xMin = -20f, _xMax = 20f, _zMin = -13f, _zMax = 29f;

    private void Start()
    {
        _speed = _moveSpeed;
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;

        _playerInput.OnMovePressed += ReceivePressedValue;
    }

    public void UpgradeMS()
    {
        _moveSpeed += 2;
        _speed = _moveSpeed;
    }

    private void ReceivePressedValue(Vector2 moveInput)
    {
        _isMoving = moveInput != Vector2.zero;

        RotateTowardsMouse();
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveDir = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * inputDirection;
        LastMoveDirection = moveDir;

        if (_isMoving)
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, _xMin, _xMax);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, _zMin, _zMax);
            transform.position = clampedPosition;
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
