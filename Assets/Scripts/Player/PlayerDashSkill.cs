using System;
using UnityEngine;

public class PlayerDashSkill : ActiveSkillBehaviour
{
    [SerializeField] private float _baseDistance = 5f;
    [SerializeField] private float _baseDuration = 0.25f;
    [SerializeField] private float _speedScale = 1f;
    private bool _dashing;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _time;
    public event Action<Vector3> OnDashStarted;
    public event Action<Vector3> OnDashEnded;
    public void SetSpeedMultiplier(float scale)
    {
        _speedScale = Mathf.Max(0.1f, scale);
        Debug.Log($"[Dash] speed scale set to {_speedScale}");
    }

    protected override void Update()
    {
        base.Update();

        if (!_dashing) return;
        
        _time += Time.deltaTime * _speedScale;
        float k = _time / _baseDuration;
        Context.transform.position = Vector3.Lerp(_startPos, _endPos, k);

        if (k >= 1f)
        {
            _dashing = false;
            OnDashEnded?.Invoke(Context.transform.position);
        }
    }

    public override void TryCast()
    {
        if (!IsReady || _dashing) return;
        base.TryCast();
        Context.Animator.Dash();
        OnDashStarted?.Invoke(Context.transform.position);
        Vector3 dir = Context.Move.LastMoveDirection;
        if (dir == Vector3.zero) dir = Context.transform.forward;

        _startPos = Context.transform.position;
        _endPos = _startPos + dir.normalized * _baseDistance;
        _time = 0f;
        _dashing = true;

        StartCooldown();
    }
}