using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchingSpireSkill : ActiveSkillBehaviour
{
     [Header("VFX / SFX")]
    [SerializeField] private GameObject _pillarPrefab;

    [Header("Damage Setup")]
    [SerializeField] private float _tickRate = 0.25f;
    private readonly List<IDamageable> _targets = new();
    public override void TryCast()
    {
        if (!IsReady) return;
        if (!GetAimPoint(out Vector3 point)) return;

        base.TryCast();

        StartCoroutine(SpireRoutine(point));
        StartCooldown();
    }
    
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

    private void DealDamage(Vector3 center, float radius, float dmgPerTick)
    {
        _targets.Clear();
        var cols = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < cols.Length; i++)
            if (cols[i].TryGetComponent(out IDamageable d) && !_targets.Contains(d))
                _targets.Add(d);

        int count = _targets.Count;
        if (count == 0) return;

        float each = dmgPerTick / count;

        for (int i = 0; i < count; i++)
        {
            var tgt = _targets[i];

            var ctx = BuildDamage(each, SkillDamageType.Basic, center, Vector3.up, gameObject);
            ctx.Target = tgt;

            // при желании ещё раз прогнать контекстные модификаторы:
            // Context.ApplyDamageContextModifiers(ref ctx);

            tgt.ReceiveDamage(ctx); // события разойдутся автоматически
        }
    }
    
    private bool GetAimPoint(out Vector3 point)
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        int groundMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(ray, out var hit, 1000f, groundMask, QueryTriggerInteraction.Ignore))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
