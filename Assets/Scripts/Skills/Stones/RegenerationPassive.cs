using System.Collections;
using UnityEngine;
public class RegenerationPassive : PassiveSkillBehaviour
{
    [Header("Regen settings")]
    [SerializeField] private float _hpPerSecond = 4f;
    [SerializeField] private float _tickRate = 0.25f;
    private Coroutine _routine;
    public override void EnablePassive()
    {
        if (_routine == null)
            _routine = StartCoroutine(RegenRoutine());
    }

    public override void DisablePassive()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private IEnumerator RegenRoutine()
    {
        var wait = new WaitForSeconds(_tickRate);
        float healPerTick = _hpPerSecond * _tickRate;

        while (true)
        {
            Context.Hp.ReceiveHP(healPerTick);
            yield return wait;
        }
    }
}