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
        PlayerContext.transform.position = Vector3.Lerp(_startPos, _endPos, k);

        if (k >= 1f) _dashing = false;
    }

    public override void TryCast()
    {
        if (!IsReady || _dashing) return;

        PlayerContext.PlayerAnimator.Dash();
        OnDashStarted?.Invoke(PlayerContext.transform.position);
        Vector3 dir = PlayerContext.PlayerMove.LastMoveDirection;
        if (dir == Vector3.zero) dir = PlayerContext.transform.forward;

        _startPos = PlayerContext.transform.position;
        _endPos = _startPos + dir.normalized * _baseDistance;
        _time = 0f;
        _dashing = true;

        StartCooldown();
    }
}