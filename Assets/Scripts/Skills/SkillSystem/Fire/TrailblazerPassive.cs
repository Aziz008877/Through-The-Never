using System.Collections;
using UnityEngine;

public class TrailblazerPassive : PassiveSkillBehaviour
{
    [Header("Trail settings")]
    [SerializeField] private FireTrailPuddle _puddlePrefab;
    [SerializeField] private float _spawnInterval = .05f;
    [SerializeField] private float _trailLifeTime = 3f;
    [SerializeField] private float _tickRate      = 0.5f;
    [SerializeField] private float _tickDamage    = 5f;
    [SerializeField] private float _radius        = 2f;

    private PlayerDashSkill     _dash;
    private PlayerSkillManager  _skillMgr;
    private Coroutine           _spawnRoutine;
    private int                 _puddleCounter;

    /* ───────── Enable / Disable ───────── */
    public override void EnablePassive()
    {
        _skillMgr = PlayerContext.PlayerSkillManager;
        AttachToDash(_skillMgr.GetActive(SkillSlot.Dash));
        _skillMgr.ActiveRegistered += OnActiveRegistered;

        Debug.Log("<color=orange>[Trailblazer]</color> enabled");
    }

    public override void DisablePassive()
    {
        _skillMgr.ActiveRegistered -= OnActiveRegistered;
        Detach();
        Debug.Log("<color=orange>[Trailblazer]</color> disabled");
    }

    /* ───────── смена Dash-скилла ───────── */
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) AttachToDash(beh);
    }

    private void AttachToDash(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashStarted += StartSpawn;
            _dash.OnDashEnded   += StopSpawn;
            Debug.Log("<color=orange>[Trailblazer]</color> attached to Dash");
        }
    }

    private void Detach()
    {
        if (!_dash) return;
        _dash.OnDashStarted -= StartSpawn;
        _dash.OnDashEnded   -= StopSpawn;
        _dash = null;
        StopSpawn(Vector3.zero);
    }

    /* ───────── спавн луж ───────── */
    private void StartSpawn(Vector3 _)
    {
        if (_spawnRoutine == null)
            _spawnRoutine = StartCoroutine(SpawnTrail());
    }

    private void StopSpawn(Vector3 _)
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
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
        var puddle = Instantiate(
            _puddlePrefab,
            PlayerContext.PlayerPosition.position,
            Quaternion.identity);

        float dmg    = PlayerContext.SkillModifierHub
                       .Apply(new SkillKey(SkillSlot.Passive, SkillStat.Damage), _tickDamage);

        float radius = PlayerContext.SkillModifierHub
                       .Apply(new SkillKey(SkillSlot.Passive, SkillStat.Radius), _radius);

        puddle.Init(dmg, _tickRate, radius, _trailLifeTime, PlayerContext);

        ++_puddleCounter;
        Debug.Log($"<color=orange>[Trailblazer]</color> puddle #{_puddleCounter} spawned");
    }

    private void OnDisable() => DisablePassive();   // гарантируем очистку
}
