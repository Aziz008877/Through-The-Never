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
        base.TryCast();
        
        if (!GetAimPoint(out Vector3 point)) return;

        StartCoroutine(SpireRoutine(point));
        StartCooldown();
    }

    private IEnumerator SpireRoutine(Vector3 center)
    {
        if (_pillarPrefab)
            Instantiate(_pillarPrefab, center, Quaternion.identity, null);

        float elapsed = 0f;
        var wait = new WaitForSeconds(_tickRate);
        
        float lifeTime = Definition.Duration > 0f ? Definition.Duration : 2f;
        float radius = Definition.Raduis > 0f ? Definition.Raduis : 3f;
        float totalDmg = Definition.Damage > 0f ? Definition.Damage : 50f;

        while (elapsed < lifeTime)
        {
            DealDamage(center, radius, totalDmg * _tickRate / lifeTime);
            elapsed += _tickRate;
            yield return wait;
        }
    }

    private void DealDamage(Vector3 center, float radius, float dmgPerTick)
    {
        _targets.Clear();
        Collider[] cols = Physics.OverlapSphere(center, radius);
        
        foreach (var col in cols)
            if (col.TryGetComponent(out IDamageable t) && !_targets.Contains(t))
                _targets.Add(t);

        if (_targets.Count == 0) return;
        
        float eachDmg = dmgPerTick / _targets.Count;
        SkillDamageType type = SkillDamageType.Basic;

        foreach (var tgt in _targets)
        {
            float dmg = eachDmg;
            PlayerContext.ApplyDamageModifiers(ref dmg, ref type);

            tgt.ReceiveDamage(dmg, type);
            PlayerContext.FireOnDamageDealt(tgt, dmg, type);
        }
    }
    
    private bool GetAimPoint(out Vector3 point)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, Definition.Range > 0 ? Definition.Range : 30f,
                Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }
}
