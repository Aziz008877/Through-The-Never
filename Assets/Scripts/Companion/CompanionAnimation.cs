using UnityEngine;

public class CompanionAnimation : MonoBehaviour, IActorAnim
{
    [SerializeField] Animator _anim;
    [SerializeField] string _moveBool = "IsMoving";
    [SerializeField] string _moveX    = "X";
    [SerializeField] string _moveY    = "Y";

    int _moveBoolHash, _moveXHash, _moveYHash;
    CompanionMove _move;

    void Awake()
    {
        _moveBoolHash = Animator.StringToHash(_moveBool);
        _moveXHash    = Animator.StringToHash(_moveX);
        _moveYHash    = Animator.StringToHash(_moveY);

        _move = GetComponent<CompanionMove>();
        if (_move) _move.OnMove += ReceiveMoveState;
    }
    void OnDestroy()
    {
        if (_move) _move.OnMove -= ReceiveMoveState;
    }

    public void Dash() => _anim.SetTrigger("Dash");   // для Dash-триггера

    /* ---------- обработка движения ---------- */
    void ReceiveMoveState(Vector3 worldDir)
    {
        bool isMoving = worldDir.sqrMagnitude > 0.0001f;

        _anim.SetBool(_moveBoolHash, isMoving);

        if (isMoving)
        {
            Vector3 localDir = transform.InverseTransformDirection(worldDir.normalized);
            _anim.SetFloat(_moveXHash, localDir.x);
            _anim.SetFloat(_moveYHash, localDir.z);
        }
        else
        {
            _anim.SetFloat(_moveXHash, 0f);
            _anim.SetFloat(_moveYHash, 0f);
        }
    }
}
