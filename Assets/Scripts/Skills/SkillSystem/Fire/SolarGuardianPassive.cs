using UnityEngine;
public sealed class SolarGuardianPassive : PassiveSkillBehaviour
{
    [Header("Guardian prefab / stats")]
    [SerializeField] private SolarGuardianOrb _guardianPrefab;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _fireInterval = 0.8f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _radius = 10f;
    private ActiveSkillBehaviour _special;
    public override void EnablePassive()
    {
        Hook(Context.SkillManager.GetActive(SkillSlot.Special));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        Unhook();
    }

    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Special) Hook(beh);
    }

    private void Hook(ActiveSkillBehaviour special)
    {
        Unhook();
        if (special != null)
        {
            _special = special;
            _special.OnCooldownStarted += _ => SpawnGuardian();
        }
    }

    private void Unhook()
    {
        if (_special != null)
        {
            _special.OnCooldownStarted -= _ => SpawnGuardian();
            _special = null;
        }
    }

    private void SpawnGuardian()
    {
        var orb = Instantiate(_guardianPrefab, Context.ActorPosition.position + Vector3.up * 1.5f, Quaternion.identity);
        float dmg = Context.SkillModifierHub.Apply(new SkillKey(SkillSlot.Passive, SkillStat.Damage), _damage);
        float radius = Context.SkillModifierHub.Apply(new SkillKey(SkillSlot.Passive, SkillStat.Radius), _radius);
        orb.Init(dmg, _fireInterval, radius, _lifeTime, Context);
    }
}
