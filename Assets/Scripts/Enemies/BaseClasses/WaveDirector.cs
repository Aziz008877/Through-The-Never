using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaveDirector : MonoBehaviour
{
    [Header("Nemesis")]
    [SerializeField] private Transform _nemesisOverrideSpawnPoint;
    [SerializeField] private int _meleeMaxTier = 4;
    [SerializeField] private int _rangedMaxTier = 3;
    private bool _nemesisAlive;

    [Header("Поток")]
    [SerializeField] private LevelFlowAsset _flow;

    [Header("Фабрика и учёт убийств")]
    [SerializeField] private EnemyFactory _factory;
    [SerializeField] private PlayerEnemyHandler _playerEnemyHandler;

    [Header("Spawn points — Layout 1 (Variant=1)")]
    [SerializeField] private List<Transform> _spawnPointsLayout1 = new();

    [Header("Spawn points — Layout 2 (Variant=2)")]
    [SerializeField] private List<Transform> _spawnPointsLayout2 = new();

    [Header("Тайминги")]
    [SerializeField] private Vector2 _delayBetweenWaves = new(2f, 3f);
    [SerializeField] private Vector2 _delayWithinWave   = new(0f, 1f);

    [Header("Выходы (по layout)")]
    [SerializeField] private ExitProp _exitLayout1;
    [SerializeField] private ExitProp _exitLayout2;
    [SerializeField] private ChestSelector _chestSelector;
    [SerializeField] private bool _lockExitWhileRunning = true;
    private List<Transform> _spawns;
    private WaveLayout _activeLayout;
    private ExitProp _activeExit;
    private int _alive;
    private readonly List<IDamageable> _aliveList = new();
    private void Start()
    {
        if (_flow == null || !_flow.TryGetCurrent(out var step))
        {
            return;
        }
        
        bool isLayout1 = step.Variant == LevelFlowAsset.LayoutVariant.First;
        _spawns     = isLayout1 ? _spawnPointsLayout1 : _spawnPointsLayout2;
        _activeExit = isLayout1 ? _exitLayout1       : _exitLayout2;

        if (_spawns == null || _spawns.Count == 0)
        {
            return;
        }

        _activeLayout = step.WaveLayout;

        if (_lockExitWhileRunning && _activeExit) _activeExit.CanInteract = false;

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        var waves = _activeLayout.Waves;

        for (int w = 0; w < waves.Length; w++)
        {
            yield return StartCoroutine(SpawnWave(waves[w]));
            while (_alive > 0) yield return null;

            if (w < waves.Length - 1)
                yield return new WaitForSeconds(Random.Range(_delayBetweenWaves.x, _delayBetweenWaves.y));
        }

        if (_lockExitWhileRunning && _activeExit) _activeExit.CanInteract = false;
        yield return StartCoroutine(TrySpawnNemesisAfterLayout());
        _chestSelector.CanInteract = true;
        _chestSelector.ActivateLootVFX();
        _exitLayout1.CanInteract = true;
        _exitLayout2.CanInteract = true;
    }
    private IEnumerator TrySpawnNemesisAfterLayout()
    {
        var svc = NemesisRuntime.Svc;

        if (!svc.TryGetActive(out var kind, out var baseTier, out var level))
        {
            yield break;
        }

        int maxTier     = (kind == EnemyKind.Melee) ? _meleeMaxTier : _rangedMaxTier;
        int desiredTier = baseTier + (level - 1);
        int targetTier  = Mathf.Clamp(desiredTier, 1, maxTier);
        int overflow    = Mathf.Max(0, desiredTier - maxTier);

        var spawnPoint = _nemesisOverrideSpawnPoint ? _nemesisOverrideSpawnPoint : GetRandomCurrentSpawnPoint();
        if (!spawnPoint) yield break;

        var enemy = _factory.Spawn(kind, targetTier, spawnPoint.position, spawnPoint.rotation);
        if (!enemy) yield break;
        
        if (overflow > 0)
        {
            enemy.MaxHP *= svc.HpResidualMult(overflow);
            enemy.CurrentHP = enemy.MaxHP;
            
            if (enemy.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent) && agent && agent.enabled)
            {
                float ms = svc.MsResidualMult(overflow);
                agent.speed *= ms;
                agent.acceleration *= Mathf.Sqrt(ms);
            }
            
            var boost = enemy.GetComponent<NemesisDamageBoost>() ?? enemy.gameObject.AddComponent<NemesisDamageBoost>();
            boost.Init(svc.DmgResidualMult(overflow));
        }
        
        _nemesisAlive = true;
        enemy.OnKilled += _ =>
        {
            _nemesisAlive = false;
            NemesisRuntime.Svc?.ClearActive();
        };

        while (_nemesisAlive) yield return null;

        if (_lockExitWhileRunning && _activeExit) _activeExit.CanInteract = true;
    }

    private Transform GetRandomCurrentSpawnPoint()
    {
        if (_nemesisOverrideSpawnPoint) return _nemesisOverrideSpawnPoint;
        if (_spawns == null || _spawns.Count == 0) return null;
        return _spawns[Random.Range(0, _spawns.Count)];
    }
    
    private IEnumerator SpawnWave(WaveLayout.Wave wave)
    {
        _alive = 0;
        _aliveList.Clear();

        int sp = 0;

        foreach (var e in wave.Entries)
        {
            for (int i = 0; i < e.Count; i++)
            {
                var p = _spawns[Random.Range(0, _spawns.Count)];
                sp++;

                var hp = _factory.Spawn(e.Kind, e.Tier, p.position, p.rotation);
                if (!hp) continue;

                IDamageable dmg = hp;
                dmg.OnEnemyDead += OnEnemyDead;
                if (_playerEnemyHandler) _playerEnemyHandler.RegisterEnemy(dmg);

                _aliveList.Add(dmg);
                _alive++;

                float d = Random.Range(_delayWithinWave.x, _delayWithinWave.y);
                if (d > 0f) yield return new WaitForSeconds(d);
            }
        }
    }


    private void OnEnemyDead(Transform who)
    {
        for (int i = 0; i < _aliveList.Count; i++)
        {
            if (_aliveList[i] is Component c && c.transform == who)
            {
                _aliveList[i].OnEnemyDead -= OnEnemyDead;
                _aliveList.RemoveAt(i);
                break;
            }
        }
        _alive = Mathf.Max(0, _alive - 1);
    }
}
