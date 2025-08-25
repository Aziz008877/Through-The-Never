using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class IceBoulderSkill : ActiveSkillBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject _boulderPrefab;
    [SerializeField] private GameObject _impactVfxPrefab;

    [Header("Cast")]
    [SerializeField] private float _maxCastDistance = 20f;
    [SerializeField] private float _spawnHeight = 12f;
    [SerializeField] private DotweenSettings _fallTween = new DotweenSettings { Duration = 0.8f, AnimationType = Ease.InQuad };

    private GameObject _boulderInstance;
    private Tween _fallTweenHandle;
    private int _lastCastFrame = -1;

    public override void TryCast()
    {
        if (!IsReady) return;
        base.TryCast();

        if (!GetMouseWorldPoint(out Vector3 targetPos))
        {
            return;
        }

        SpawnAndFall(targetPos);
    }

    private bool GetMouseWorldPoint(out Vector3 point)
    {
        point = Vector3.zero;

        Camera cam = Context.MainCamera;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxCastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 p = hit.point;

            if (NavMesh.SamplePosition(p, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                p = navHit.position;

            point = p;
            return true;
        }

        float planeY = Context.transform.position.y;
        Plane ground = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));

        if (ground.Raycast(ray, out float enter))
        {
            Vector3 p = ray.GetPoint(enter);

            if (NavMesh.SamplePosition(p, out NavMeshHit navHit, 3f, NavMesh.AllAreas))
            {
                point = navHit.position;
                return true;
            }
            else
            {
                point = p;
                return true;
            }
        }

        return false;
    }
    
    private void SpawnAndFall(Vector3 targetPos)
    {
        if (_boulderPrefab == null)
        {
            Impact(targetPos);
            return;
        }

        if (_boulderInstance != null) Destroy(_boulderInstance);
        if (_fallTweenHandle != null && _fallTweenHandle.IsActive()) _fallTweenHandle.Kill();

        Vector3 spawnPos = targetPos + Vector3.up * _spawnHeight;
        _boulderInstance = Instantiate(_boulderPrefab, spawnPos, Quaternion.identity);

        _fallTweenHandle = _boulderInstance.transform
            .DOMove(targetPos, Mathf.Max(0.01f, _fallTween.Duration))
            .SetEase(_fallTween.AnimationType)
            .SetLink(_boulderInstance)
            .OnComplete(() =>
            {
                Impact(targetPos);
            });
    }

    private void Impact(Vector3 pos)
    {
        var explosion = Instantiate(_impactVfxPrefab, pos, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        var hits = Physics.OverlapSphere(pos, Radius, ~0, QueryTriggerInteraction.Collide);

        float dealt = 0f;
        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (!col.TryGetComponent(out IDamageable tgt)) continue;

            var ctx = BuildDamage(Damage, SkillDamageType.Basic,
                hitPoint: col.transform.position,
                hitNormal: Vector3.up,
                sourceGO: gameObject);
            ctx.Target = tgt;

            tgt.ReceiveDamage(ctx);      // события разойдутся внутри цели
            dealt += ctx.Damage;         // фактический урон после модификаторов/критов
        }

        if (_boulderInstance) Destroy(_boulderInstance);
        StartCooldown();
    }
}
