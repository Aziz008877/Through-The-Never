using System.Collections;
using UnityEngine;
public class HailstormSkill : ActiveSkillBehaviour
{
    [SerializeField] private HailSpikeProjectile _spike;
    [SerializeField] private float _spikeCount, _spawnDelay, _spawnHeight;
    [SerializeField] private DotweenSettings _spikeFallDotween;
    private Vector3 _point;
    public override void TryCast()
    {
        if (!IsReady) return;
        _point = GetAimPoint();
        StartCoroutine(SpawnCoroutine(_point));
        StartCooldown();
    }
    
    private Vector3 GetAimPoint()
    {
        Camera cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 point = Vector3.zero;
        if (Physics.Raycast(ray, out RaycastHit hit, Definition.Range > 0 ? Definition.Range : 30f))
        {
            point = hit.point;
            return point;
        }

        point = Vector3.zero;
        return point;
    }

    private IEnumerator SpawnCoroutine(Vector3 center)
    {
        for (int i = 0; i < _spikeCount; i++)
        {
            var offset = Random.insideUnitCircle * Radius;
            var targetPos = center + new Vector3(offset.x, 0f, offset.y);
            var spawnPos = targetPos + Vector3.up * _spawnHeight;

            var spike = Instantiate(_spike, spawnPos, Quaternion.identity);
            spike.Init(spawnPos, _spikeFallDotween, Damage, Radius, Context);

            yield return new WaitForSeconds(_spawnDelay);
        }
    }
}
