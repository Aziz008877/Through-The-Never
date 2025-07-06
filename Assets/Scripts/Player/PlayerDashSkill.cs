using UnityEngine;

public class PlayerDashSkill : ActiveSkillBehaviour
{
    [SerializeField] private float _baseDistance = 5f;
    [SerializeField] private float _speedMultiplier = 0.1f;
    private bool _isDashing;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _elapsedTime;

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.1f, multiplier);
        Debug.Log($"[Dash] speed multiplier set to {_speedMultiplier}");
    }

    protected override void Update()
    {
        base.Update();

        if (!_isDashing) return;

        _elapsedTime += Time.deltaTime * _speedMultiplier;
        float k = _elapsedTime / Definition.Duration;
        PlayerContext.transform.position = Vector3.Lerp(_startPosition, _endPosition, k);

        if (k >= 1f) _isDashing = false;
    }

    public override void TryCast()
    {
        if (!IsReady || _isDashing) return;

        PlayerContext.PlayerAnimator.Dash();

        Vector3 direction = PlayerContext.PlayerMove.LastMoveDirection;
        if (direction == Vector3.zero) direction = PlayerContext.transform.forward;

        _startPosition = PlayerContext.transform.position;
        _endPosition = _startPosition + direction.normalized * _baseDistance;
        _elapsedTime = 0f;
        _isDashing = true;

        StartCooldown();
    }
}