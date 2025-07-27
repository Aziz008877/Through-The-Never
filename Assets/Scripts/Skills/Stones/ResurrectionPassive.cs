using System;
using UnityEngine;

public class ResurrectionPassive : PassiveSkillBehaviour
{
    [SerializeField] private float _healPercent = 1f;
    private bool _used;
    public Action<ResurrectionPassive> OnResurrected;

    public override void EnablePassive()
    {
        PlayerContext.PlayerHp.OnPlayerDead += OnPlayerDead;
    }

    public override void DisablePassive()
    {
        PlayerContext.PlayerHp.OnPlayerDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        if (_used) return;
        _used = true;
        PlayerContext.PlayerHp.Revive(_healPercent);
        OnResurrected?.Invoke(this);
    }
}