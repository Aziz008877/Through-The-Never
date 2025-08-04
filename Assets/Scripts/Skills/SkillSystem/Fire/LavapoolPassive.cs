using UnityEngine;

public sealed class LavapoolPassive : PassiveSkillBehaviour
{
    [Header("Lavapool")]
    [SerializeField] private LavaPoolArea _lavaPrefab;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] private float _dps = 8f;
    [SerializeField] private float _radius = 2.2f;
    private PlayerDashSkill _dash;
    public override void EnablePassive()
    {
        Attach(Context.SkillManager.GetActive(SkillSlot.Dash));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Detach();
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();

        if (beh && beh.TryGetComponent(out PlayerDashSkill dash))
        {
            _dash = dash;
            _dash.OnDashEnded += SpawnLavapool;
        }
    }

    private void Detach()
    {
        if (!_dash) return;
        _dash.OnDashEnded -= SpawnLavapool;
        _dash = null;
    }
    
    private void SpawnLavapool(Vector3 endPos)
    {
        var pool = Instantiate(_lavaPrefab, endPos, Quaternion.identity);
        pool.Init(_dps, _radius, _lifeTime, Context);

        Debug.Log(
            $"<color=orange>[Lavapool]</color> spawned at {endPos} " +
            $"({_dps} DPS, {_lifeTime}s, r={_radius})");
    }
}