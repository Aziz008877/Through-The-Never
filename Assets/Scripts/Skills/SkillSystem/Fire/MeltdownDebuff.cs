using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BaseEnemyHP))]
public class MeltdownDebuff : MonoBehaviour
{
    public  int StackCount { get; private set; }
    private readonly int _maxStacks = 5;

    /* параметры, получает из Configure() */
    private float            _stackLife;
    private FireTrailPuddle  _puddlePrefab;
    private float            _puddleDps, _puddleRate, _puddleRad, _puddleLife;
    private ActorContext    _ctx;

    /* внутреннее состояние */
    private Coroutine  _decayRoutine;
    private Coroutine  _trailRoutine;

    /* ───────── настройка (вызывается единожды) ───────── */
    public void Configure(float stackLife, FireTrailPuddle prefab, float dps, float rate, float radius, float life, ActorContext ctx)
    {
        _stackLife     = stackLife;
        _puddlePrefab  = prefab;
        _puddleDps     = dps;
        _puddleRate    = rate;
        _puddleRad     = radius;
        _puddleLife    = life;
        _ctx           = ctx;
    }

    /* ───────── +1 стак ───────── */
    public void AddStack()
    {
        StackCount = Mathf.Min(StackCount + 1, _maxStacks);

        /* обновляем таймер распада */
        if (_decayRoutine != null) StopCoroutine(_decayRoutine);
        _decayRoutine = StartCoroutine(Decay());

        /* если достигли 5 стаков — запускаем лаву */
        if (StackCount >= _maxStacks && _trailRoutine == null)
            _trailRoutine = StartCoroutine(SpawnTrail());
    }

    /* ───────── распад стаков ───────── */
    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(_stackLife);
        StackCount = Mathf.Max(0, StackCount - 1);
        Debug.Log($"<color=orange>[Meltdown]</color> stacks decay → {StackCount}");

        if (StackCount > 0)
            _decayRoutine = StartCoroutine(Decay());
        else
            StopTrail();
    }

    /* ───────── лавовый след ───────── */
    private IEnumerator SpawnTrail()
    {
        var wait = new WaitForSeconds(_puddleRate);
        Debug.Log($"<color=orange>[Meltdown]</color> TRAIL START ({name})");

        while (StackCount >= _maxStacks)
        {
            var p = Instantiate(_puddlePrefab, transform.position, Quaternion.identity);
            p.Init(_puddleDps, _puddleRate, _puddleRad, _puddleLife, _ctx);
            yield return wait;
        }
        Debug.Log($"<color=orange>[Meltdown]</color> trail stop ({name})");
    }

    private void StopTrail()
    {
        if (_trailRoutine != null)
        {
            StopCoroutine(_trailRoutine);
            _trailRoutine = null;
        }
    }

    private void OnDisable() => StopTrail();
}
