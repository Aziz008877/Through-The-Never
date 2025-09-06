using System.Collections;
using UnityEngine;

public class HailstormSkill : ActiveSkillBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private HailSpikeProjectile _spike;

    [Header("Spawn")]
    [SerializeField] private int   _spikeCount   = 12;
    [SerializeField] private float _spawnDelay   = 0.06f;
    [SerializeField] private float _spawnHeight  = 10f;

    [Header("Tween")]
    [SerializeField] private DotweenSettings _spikeFallDotween;

    public override void TryCast()
    {
        if (!IsReady) return;

        Vector3 center = GetAimPoint();
        StartCoroutine(SpawnCoroutine(center));
        StartCooldown();
    }

    private Vector3 GetAimPoint()
    {
        var cam = (Context && Context.MainCamera) ? Context.MainCamera : Camera.main;
        if (!cam)
        {
            Vector3 origin = Context && Context.CastPivot ? Context.CastPivot.position : transform.position;
            Vector3 fwd    = Context && Context.CastPivot ? Context.CastPivot.forward  : transform.forward;
            float   range  = Definition.Range > 0 ? Definition.Range : 10f;
            return origin + fwd * range * 0.7f;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float rangeClamp = Definition.Range > 0 ? Definition.Range : 30f;
        
        if (Physics.Raycast(ray, out var hit, rangeClamp))
            return ClampToRange(hit.point, rangeClamp);
        
        Vector3 planePoint = Context ? Context.transform.position : Vector3.zero;
        var plane = new Plane(Vector3.up, planePoint);
        if (plane.Raycast(ray, out float dist))
            return ClampToRange(ray.GetPoint(dist), rangeClamp);
        
        return ClampToRange(planePoint + (Context ? Context.transform.forward : Vector3.forward) * rangeClamp, rangeClamp);
    }

    private Vector3 ClampToRange(Vector3 worldPoint, float maxRange)
    {
        Vector3 origin = Context && Context.CastPivot ? Context.CastPivot.position : transform.position;
        Vector3 v = worldPoint - origin; v.y = 0f;
        float d = v.magnitude;
        if (d > maxRange && d > 0.001f) return origin + v / d * maxRange;
        return worldPoint;
    }

    private IEnumerator SpawnCoroutine(Vector3 center)
    {
        float r = Radius;

        for (int i = 0; i < _spikeCount; i++)
        {
            Vector2 offset2 = Random.insideUnitCircle * r;
            Vector3 targetPos = center + new Vector3(offset2.x, 0f, offset2.y);
            Vector3 spawnPos  = targetPos + Vector3.up * _spawnHeight;

            var spike = Instantiate(_spike, spawnPos, Quaternion.identity);
            spike.Init(targetPos, _spikeFallDotween, Damage, r, Context);

            if (_spawnDelay > 0f) yield return new WaitForSeconds(_spawnDelay);
            else yield return null;
        }
    }
}
