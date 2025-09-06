using System.Collections;
using UnityEngine;

public class DeepfrostWatersSkill : ActiveSkillBehaviour
{
    [SerializeField] private float _duration = 15f;
    [SerializeField] private float _cooldown = 30f;

    private Coroutine _routine;
    private bool _active;

    public override void TryCast()
    {
        // Тумблер: если активен — выключаем и уходим на КД; если нет — включаем (если готов).
        if (_active)
        {
            Deactivate(startCd: true);
            return;
        }

        if (!IsReady) return;
        Activate(); // ВАЖНО: КД не трогаем при включении
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
        float t = _duration;
        while (_active && t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        // Авто-выключение по таймеру → КД
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
        if (_active) Deactivate(startCd: false); // выгрузка без постановки КД
    }
}