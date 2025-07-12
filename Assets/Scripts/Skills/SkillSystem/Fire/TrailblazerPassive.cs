using System.Collections;
using UnityEngine;
public class TrailblazerPassive : PassiveSkillBehaviour
{
    [Header("Trail settings")]
    [SerializeField] private FireTrailPuddle _puddlePrefab;
    [SerializeField] private float _spawnInterval = .05f;
    [SerializeField] private float _trailLifeTime = 3f;
    [SerializeField] private float _tickRate = .5f;
    [SerializeField] private float _tickDamage = 5f;
    [SerializeField] private float _radius = 2f;
    private PlayerDashSkill _dash;
    private PlayerSkillManager _playerSkillManager;
    private Coroutine _spawnRoutine;
    public override void EnablePassive()
    {
        _playerSkillManager = PlayerContext.PlayerSkillManager;
        AttachToDash(_playerSkillManager.GetActive(SkillSlot.Dash));
        _playerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        _playerSkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash)
        {
            AttachToDash(beh);
        }
    }

    private void AttachToDash(ActiveSkillBehaviour beh)
    {
        Detach();
        
        if (beh != null && beh.TryGetComponent<PlayerDashSkill>(out var dash))
        {
            _dash = dash;
            _dash.OnDashStarted += StartSpawn;
            _dash.OnDashEnded += StopSpawn;
        }
    }

    private void Detach()
    {
        if (_dash != null)
        {
            _dash.OnDashStarted -= StartSpawn;
            _dash.OnDashEnded -= StopSpawn;
            _dash = null;
        }
        
        StopSpawn(PlayerContext.transform.position);
    }
    
    private void StartSpawn(Vector3 dashPosition)
    {
        _spawnRoutine ??= StartCoroutine(SpawnTrail());
    }

    private void StopSpawn(Vector3 dashPosition)
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine); _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnTrail()
    {
        var wait = new WaitForSeconds(_spawnInterval);
        while (true)
        {
            SpawnPuddle();
            yield return wait;
        }
    }

    private void SpawnPuddle()
    {
        var puddle = Instantiate(_puddlePrefab, PlayerContext.PlayerPosition.position, Quaternion.identity, null);
        float dmg = PlayerContext.SkillModifierHub.Apply(new SkillKey(SkillSlot.Passive, SkillStat.Damage), _tickDamage);
        float radius = PlayerContext.SkillModifierHub.Apply(new SkillKey(SkillSlot.Passive, SkillStat.Radius), _radius);
        puddle.Init(dmg, _tickRate, radius, _trailLifeTime, PlayerContext);
    }
}
