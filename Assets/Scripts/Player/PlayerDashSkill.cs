using UnityEngine;

public class PlayerDashSkill : ActiveSkillBehaviour
{
    [SerializeField] private float _distance = 5f;
    private bool _dashing;
    private Vector3 _start;
    private Vector3 _end;
    private float _timer;

    private void Update()
    {
        base.Update();                          // << обязательно, чтобы кулдаун считал

        if (!_dashing) return;

        _timer += Time.deltaTime;
        float k = _timer / Definition.Duration;
        PlayerContext.transform.position = Vector3.Lerp(_start, _end, k);

        if (k >= 1f) _dashing = false;
    }


    public override void TryCast()
    {
        if (!IsReady || _dashing) return;
        PlayerContext.PlayerAnimator.Dash();
        Vector3 dir = PlayerContext.PlayerMove.LastMoveDirection;
        if (dir == Vector3.zero) dir = PlayerContext.transform.forward;

        _start = PlayerContext.transform.position;
        _end = _start + dir.normalized * _distance;
        _timer = 0f;
        _dashing = true;

        StartCooldown();
    }
}