using System.Collections;
using UnityEngine;

public class DeepfrostWatersSkill : ActiveSkillBehaviour
{
    private Coroutine _routine;
    private bool _active;

    public override void TryCast()
    {
        if (_active)
        {
            Deactivate(startCd: true);
            return;
        }

        if (!IsReady) return;
        Activate();
    }

    private void Activate()
    {
        _active = true;
        Context.DeepfrostWaterMode = true;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(DurationRoutine());
    }

    private IEnumerator DurationRoutine()
    {
        float t = Definition.Duration;
        while (_active && t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        
        if (_active) Deactivate(startCd: true);
    }

    public void ForceDeactivate() => Deactivate(startCd: true);

    private void Deactivate(bool startCd)
    {
        if (!_active) return;

        _active = false;
        Context.DeepfrostWaterMode = false;

        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (startCd)
            StartCooldown();
    }

    private void OnDisable()
    {
        if (_active) Deactivate(startCd: false);
    }
}