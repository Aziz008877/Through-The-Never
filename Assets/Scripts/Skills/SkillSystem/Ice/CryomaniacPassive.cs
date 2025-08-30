using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CryomaniacPassive : PassiveSkillBehaviour
{
    [SerializeField] private bool _debugLog;

    private readonly List<BaseEnemyHP> _enemySubs = new();
    private IEnemyHandler _enemyHandler;
    private ActiveSkillBehaviour _dash;
    public override void EnablePassive()
    {
        HookDash(Context.SkillManager.GetActive(SkillSlot.Dash));
        Context.SkillManager.ActiveRegistered += OnActiveRegistered;
        
        _enemyHandler = Context.EnemyHandler;
        
        if (_enemyHandler != null)
        {
            _enemyHandler.EnemyRegistered += OnEnemyRegistered;
            var existing = _enemyHandler.Enemies;
            if (existing != null && existing.Length > 0)
            {
                for (int i = 0; i < existing.Length; i++) TrySubEnemy(existing[i]);
            }
            else
            {
                var inScene = Context.EnemyHandler.Enemies;
                for (int i = 0; i < inScene.Length; i++) TrySubEnemy(inScene[i]);
            }
        }
        else
        {
            var inScene = Context.EnemyHandler.Enemies;
            for (int i = 0; i < inScene.Length; i++) TrySubEnemy(inScene[i]);
        }
    }

    public override void DisablePassive()
    {
        Context.SkillManager.ActiveRegistered -= OnActiveRegistered;
        UnhookDash();

        if (_enemyHandler != null)
        {
            _enemyHandler.EnemyRegistered -= OnEnemyRegistered;
            _enemyHandler = null;
        }

        for (int i = 0; i < _enemySubs.Count; i++)
            if (_enemySubs[i] != null) _enemySubs[i].OnKilled -= OnEnemyKilled;

        _enemySubs.Clear();
    }
    
    private void OnActiveRegistered(SkillSlot slot, ActiveSkillBehaviour beh)
    {
        if (slot == SkillSlot.Dash) HookDash(beh);
    }

    private void HookDash(ActiveSkillBehaviour dash)
    {
        UnhookDash();
        if (dash != null) _dash = dash;
    }

    private void UnhookDash()
    {
        _dash = null;
    }
    
    private void OnEnemyRegistered(IDamageable dmg)
    {
        if (dmg is BaseEnemyHP hp) TrySubEnemy(hp);
    }

    private void TrySubEnemy(BaseEnemyHP hp)
    {
        if (hp == null || _enemySubs.Contains(hp)) return;
        hp.OnKilled += OnEnemyKilled;
        _enemySubs.Add(hp);
    }

    private void OnEnemyKilled(DamageContext ctx)
    {
        if (ctx.Attacker != Context) return;
        if (ctx.Slot != SkillSlot.Dash) return;

        ResetAllCooldowns();

        if (_debugLog)
            Debug.Log("<color=#66d0ff>[Cryomaniac]</color> Dash kill → reset ALL cooldowns");
    }
    private void ResetAllCooldowns()
    {
        var mgr = Context.SkillManager;
        if (mgr == null) return;
        
        foreach (SkillSlot slot in Enum.GetValues(typeof(SkillSlot)))
        {
            var active = mgr.GetActive(slot);
            if (active != null)
                active.SetCooldown(0f);
        }
    }
}
