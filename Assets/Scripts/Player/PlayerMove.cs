using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCamera;
    public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;

    private PlayerInput _playerInput;
    private float _speedMultiplier = 1f;
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

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    public void UpgradeMS(float speed)
    {
        _moveSpeed += 2;
        _speed = _moveSpeed;
    }

    private void ReceivePressedValue(Vector2 moveInput)
    {
        _isMoving = moveInput != Vector2.zero;

        Vector3 camForward = _mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = _mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camRight * moveInput.x + camForward * moveInput.y;
        LastMoveDirection = moveDir;

        if (_isMoving)
        {
            transform.position += moveDir.normalized * _speed * _speedMultiplier * Time.deltaTime;

            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, _xMin, _xMax);
            p.z = Mathf.Clamp(p.z, _zMin, _zMax);
            transform.position = p;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * _rotationSpeed);
            }
        }

        OnPlayerMove?.Invoke(moveDir);
    }
    
    public void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float dist))
        {
            Vector3 look = ray.GetPoint(dist) - transform.position;
            look.y = 0f;

            if (look.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(look);
            }
        }
    }
}
