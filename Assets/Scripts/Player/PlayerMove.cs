using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IActorMove, IExternalSpeedMul
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _aimLockDuration = 0.25f;
    [SerializeField] private float _xMin = -7.5f, _xMax = 5f, _zMin = -13f, _zMax = 20f;
    private float _aimLockTimer;
    public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;

    private PlayerInput _playerInput;
    private float _speed;
    private bool _isMoving;
    private bool _isSprinting;
    private bool _isCrouching;
    public Action<Vector3> OnPlayerMove;
    private float _baseSpeedMul = 1f;
    private float _externalSpeedMul = 1f;
    private readonly Dictionary<object, float> _externalMuls = new();

    private void Start()
    {
        _speed = _moveSpeed;
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;

        _playerInput.OnMovePressed += ReceivePressedValue;
    }
    public void SetSpeedMultiplier(float multiplier)
    {
        _baseSpeedMul = Mathf.Max(0.1f, multiplier);
        RecomputeExternalMul();
    }

    public void UpgradeMS(float speed)
    {
        _moveSpeed += 2;
        _speed = _moveSpeed;
    }
    
    public void PushExternalSpeedMul(object key, float mul)
    {
        Debug.Log(key);
        Debug.Log(mul);
        if (key == null) return;
        if (mul <= 0f) mul = 1f;
        _externalMuls[key] = mul;
        Debug.Log(mul);
        RecomputeExternalMul();
    }

    public void PopExternalSpeedMul(object key)
    {
        if (key == null) return;
        if (_externalMuls.Remove(key))
            RecomputeExternalMul();
    }

    private void RecomputeExternalMul()
    {
        float prod = 1f;
        foreach (var kv in _externalMuls) prod *= kv.Value;
        _externalSpeedMul = Mathf.Max(0.1f, prod);
    }

    private float EffectiveSpeed => _speed * _baseSpeedMul * _externalSpeedMul;

    private void ReceivePressedValue(Vector2 moveInput)
    {
        _isMoving = moveInput != Vector2.zero;

        if (_aimLockTimer > 0f) _aimLockTimer -= Time.deltaTime;

        Vector3 camF = _mainCamera.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = _mainCamera.transform.right;   camR.y = 0; camR.Normalize();

        Vector3 moveDir = camR * moveInput.x + camF * moveInput.y;
        LastMoveDirection = moveDir;

        if (_isMoving)
        {
            transform.position += moveDir.normalized * EffectiveSpeed * Time.deltaTime;

            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, _xMin, _xMax);
            p.z = Mathf.Clamp(p.z, _zMin, _zMax);
            transform.position = p;

            if (_aimLockTimer <= 0f && moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * _rotationSpeed);
            }
        }

        OnPlayerMove?.Invoke(moveDir);
    }

    public void RotateTowardsMouse(float customDuration = -1f)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float dist))
        {
            Vector3 look = ray.GetPoint(dist) - transform.position;
            look.y = 0f;

            if (look.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(look);
        }

        _aimLockTimer = customDuration > 0f ? customDuration : _aimLockDuration;
    }
}

public interface IExternalSpeedMul
{
    void PushExternalSpeedMul(object key, float mul);
    void PopExternalSpeedMul(object key);
}
