using UnityEngine;
public sealed class SmolderRingPassive : PassiveSkillBehaviour
{
    [SerializeField] private SmolderRingArea _ringPrefab;
    private PlayerDashSkill _dash;
    private PlayerSkillManager _playerSkillManager;
    public override void EnablePassive()
    {
        _playerSkillManager = PlayerContext.PlayerSkillManager;
        Attach(_playerSkillManager.GetActive(SkillSlot.Dash));
        _playerSkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        _playerSkillManager.ActiveRegistered -= OnActiveRegistered;
        Detach();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) Attach(beh);
    }

    private void Attach(ActiveSkillBehaviour beh)
    {
        Detach();
        if (beh != null && beh.TryGetComponent<PlayerDashSkill>(out _dash))
            _dash.OnDashEnded += SpawnRing;
    }

    private void Detach()
    {
        if (_dash != null) _dash.OnDashEnded -= SpawnRing;
        _dash = null;
    }

    private void SpawnRing(Vector3 dashEndPosition)
    {
        var ring = Instantiate(_ringPrefab, dashEndPosition, Quaternion.identity);
        ring.Init(PlayerContext);
    }
}