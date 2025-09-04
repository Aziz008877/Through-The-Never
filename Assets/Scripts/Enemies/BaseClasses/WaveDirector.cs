using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaveDirector : MonoBehaviour
{
    [Header("Поток")]
    [SerializeField] private LevelFlowAsset _flow;

    [Header("Фабрика и учёт убийств")]
    [SerializeField] private EnemyFactory _factory;
    [SerializeField] private PlayerEnemyHandler _playerEnemyHandler;

    [Header("Spawn points — Layout 1 (Variant=1)")]
    [SerializeField] private Transform _spawnRootLayout1;
    [SerializeField] private List<Transform> _spawnPointsLayout1 = new();

    [Header("Spawn points — Layout 2 (Variant=2)")]
    [SerializeField] private Transform _spawnRootLayout2;
    [SerializeField] private List<Transform> _spawnPointsLayout2 = new();

    [Header("Тайминги")]
    [SerializeField] private Vector2 _delayBetweenWaves = new(2f, 3f);
    [SerializeField] private Vector2 _delayWithinWave   = new(0f, 1f);

    [Header("Выходы (по layout)")]
    [SerializeField] private ExitProp _exitLayout1;
    [SerializeField] private ExitProp _exitLayout2;
    [SerializeField] private bool _lockExitWhileRunning = true;
    private List<Transform> _spawns;
    private WaveLayout _activeLayout;
    private ExitProp _activeExit;
    private int _alive;
    private readonly List<IDamageable> _aliveList = new();
    [SerializeField] private WaveLayout _debugOverrideLayout;
    private void Start()
    {
        if (_flow == null || !_flow.TryGetCurrent(out var step))
        {
            Debug.LogError("[WaveDirector] Нет текущего шага потока.");
            return;
        }
        
        bool isLayout1 = step.Variant == LevelFlowAsset.LayoutVariant.First;
        _spawns     = isLayout1 ? _spawnPointsLayout1 : _spawnPointsLayout2;
        _activeExit = isLayout1 ? _exitLayout1       : _exitLayout2;

        if (_spawns == null || _spawns.Count == 0)
        {
            Debug.LogError("[WaveDirector] Нет точек спавна для текущего layout.");
            return;
        }

        _activeLayout = _debugOverrideLayout ? _debugOverrideLayout : step.WaveLayout;

        Debug.Log($"[WaveDirector] FLOW: step picked -> variant={step.Variant}, " +
                  $"layout={( _activeLayout ? _activeLayout.name : "NULL")}");

        /*if (_activeLayout && _activeLayout.Waves != null)
        {
            for (int w = 0; w < _activeLayout.Waves.Length; w++)
            {
                foreach (var e in _activeLayout.Waves[w].Entries)
                    Debug.Log($"[WaveDirector] Layout '{_activeLayout.name}', wave {w}: {e.Kind} x{e.Count} (tier {e.Tier})");
            }
        }*/

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

        if (_activeExit) _activeExit.CanInteract = true;
        Debug.Log("[WaveDirector] Все волны пройдены (для текущего layout).");
    }

    private IEnumerator SpawnWave(WaveLayout.Wave wave)
    {
        _alive = 0;
        _aliveList.Clear();

        int sp = 0;
        int spCount = Mathf.Max(1, _spawns.Count);

        // Лог структуры волны
        foreach (var e in wave.Entries)
            Debug.Log($"[WaveDirector] Spawn request: {e.Kind} x{e.Count} (tier {e.Tier})");

        foreach (var e in wave.Entries)
        {
            for (int i = 0; i < e.Count; i++)
            {
                var p = _spawns[sp % spCount];
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

    private static void CollectChildren(Transform root, List<Transform> into)
    {
        /*into.Clear();
        foreach (Transform t in root) into.Add(t);
        if (into.Count == 0) into.Add(root);*/
    }
}
