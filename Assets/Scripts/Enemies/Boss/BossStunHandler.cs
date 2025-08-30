using System.Collections;
using UnityEngine;
public sealed class BossStunHandler : MonoBehaviour, IStunnable
{
    private bool _isStunned;
    private BaseEnemyMove _move;
    private SandBossAttack _attack;
    private Coroutine _co;

    public bool IsStunned => _isStunned;

    private void Awake()
    {
        _move = GetComponent<BaseEnemyMove>();
        _attack = GetComponent<SandBossAttack>();
    }

    public void ApplyStun(float duration)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _isStunned = true;
        _attack?.InterruptCast();
        _move.SetMoveState(false);

        float t = duration;
        while (t > 0f) { t -= Time.deltaTime; yield return null; }

        _move.SetMoveState(true);
        _isStunned = false;
        _co = null;
    }
}