using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchingSpireSkill : ActiveSkillBehaviour
{
     [Header("VFX / SFX")]
    [SerializeField] private GameObject _pillarPrefab;      // prefab, а не объект в сцене

    [Header("Damage Setup")]
    [SerializeField] private float _tickRate = 0.25f;

    private readonly List<IDamageable> _targets = new();

    /* ─────────────────────────────── PUBLIC API ────────────────────────────── */
    public override void TryCast()
    {
        if (!IsReady) return;                       // кулдаун
        if (!GetAimPoint(out Vector3 point)) return;

        base.TryCast();                             // бросаем событие «активирован»

        StartCoroutine(SpireRoutine(point));
        StartCooldown();
    }

    /* ─────────────────────────────── MAIN ROUTINE ─────────────────────────── */
    private IEnumerator SpireRoutine(Vector3 center)
    {
        // 1) создаём визуал
        GameObject pillar = _pillarPrefab
            ? Instantiate(_pillarPrefab, center, Quaternion.identity)
            : null;

        // 2) параметры из Definition c fallback-ами
        float lifeTime  = Definition.Duration > 0f ? Definition.Duration : 2f;
        float radius    = Definition.Raduis   > 0f ? Definition.Raduis   : 3f;
        float totalDmg  = Definition.Damage   > 0f ? Definition.Damage   : 50f;

        // 3) наносим урон по тик-рейту
        float elapsed = 0f;
        var   wait    = new WaitForSeconds(_tickRate);

        while (elapsed < lifeTime)
        {
            DealDamage(center, radius, totalDmg * _tickRate / lifeTime);
            elapsed += _tickRate;
            yield return wait;
        }

        // 4) убираем визуал и саму «спелл-капсулу»
        if (pillar) Destroy(pillar);
        Destroy(gameObject);
    }

    /* ───────────────────────────── DAMAGE ─────────────────────────────────── */
    private void DealDamage(Vector3 center, float radius, float dmgPerTick)
    {
        _targets.Clear();
        Collider[] cols = Physics.OverlapSphere(center, radius);

        foreach (var col in cols)
            if (col.TryGetComponent(out IDamageable d) && !_targets.Contains(d))
                _targets.Add(d);

        if (_targets.Count == 0) return;

        float each = dmgPerTick / _targets.Count;
        SkillDamageType type = SkillDamageType.Basic;

        foreach (var tgt in _targets)
        {
            float dmg = each;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);
            tgt.ReceiveDamage(dmg, type);
            PlayerContext.FireOnDamageDealt(tgt, dmg, type);
        }
    }

    /* ───────────────────────────── AIM POINT ──────────────────────────────── */
    private bool GetAimPoint(out Vector3 point)
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        int groundMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(ray, out var hit, 1000f, groundMask,
                            QueryTriggerInteraction.Ignore))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
