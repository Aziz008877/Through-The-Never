using System;
using UnityEngine;

public class ResurrectionPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _healPercent = 1f;
    private bool _used;
    public Action<ResurrectionPassive> OnResurrected;

    public override void EnablePassive()
    {
        Context.Hp.OnActorDead += OnPlayerDead;
    }

    public override void DisablePassive()
    {
        Context.Hp.OnActorDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        if (_used) return;
        _used = true;
        Context.Hp.Revive(_healPercent);
        OnResurrected?.Invoke(this);
    }
}