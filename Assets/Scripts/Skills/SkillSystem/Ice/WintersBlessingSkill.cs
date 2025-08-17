using UnityEngine;

public class WintersBlessingSkill : PassiveSkillBehaviour
{
    [SerializeField] private float _percentOfTotalEnemyMaxHp = 0.08f;
    [SerializeField] private float _maxShieldCap = 300f;
    [SerializeField] private GameObject _barrierVfx;
    [SerializeField] private float _shieldLeft;
    private bool _activated;

    public override void EnablePassive()
    {
        Context.Hp.OnIncomingDamage += OnIncomingDamage;
    }

    public override void DisablePassive()
    {
        Context.Hp.OnIncomingDamage -= OnIncomingDamage;
        DeactivateVfx();
        _activated = false;
        _shieldLeft = 0f;
    }

    private void OnIncomingDamage(ref float damage, IDamageable source)
    {
        if (!_activated)
        {
            float total = 0f;
            foreach (var enemy in Context.EnemyHandler.Enemies)
            {
                total += enemy.MaxHP;
            }

            _shieldLeft = Mathf.Min(total * _percentOfTotalEnemyMaxHp, _maxShieldCap);
            if (_shieldLeft > 0f) ActivateVfx();
            _activated = true;
        }

        if (_shieldLeft <= 0f || damage <= 0f) return;

        float absorbed = Mathf.Min(_shieldLeft, damage);
        damage -= absorbed;
        _shieldLeft -= absorbed;

        if (_shieldLeft <= 0f) DeactivateVfx();
    }

    private void ActivateVfx()
    {
        _barrierVfx.SetActive(true);
    }

    private void DeactivateVfx()
    {
        _barrierVfx.SetActive(false);
    }
}
