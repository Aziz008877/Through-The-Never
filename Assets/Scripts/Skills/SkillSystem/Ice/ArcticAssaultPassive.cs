using System.Collections.Generic;
using UnityEngine;

public sealed class ArcticAssaultPassive : PassiveSkillBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _maxHpPerKill = 5f;
    [SerializeField] private bool  _healByGain   = true;

    private readonly List<BaseEnemyHP> _subs = new();
    private IEnemyHandler _handler;

    public override void EnablePassive()
    {
        // 1) подписываемся на уже существующих врагов
        var enemies = Context.EnemyHandler.Enemies;
        for (int i = 0; i < enemies.Length; i++) TrySubscribe(enemies[i]);

        // 2) слушаем новых через IEnemyHandler.EnemyRegistered (он у тебя уже есть)
        _handler = Context.EnemyHandler;
        if (_handler != null)
            _handler.EnemyRegistered += OnEnemyRegistered;
    }

    public override void DisablePassive()
    {
        if (_handler != null)
            _handler.EnemyRegistered -= OnEnemyRegistered;

        // отписываемся от всех врагов
        for (int i = 0; i < _subs.Count; i++)
            if (_subs[i] != null) _subs[i].OnKilled -= OnEnemyKilled;

        _subs.Clear();
        _handler = null;
    }

    private void OnEnemyRegistered(IDamageable dmg)
    {
        if (dmg is BaseEnemyHP hp) TrySubscribe(hp);
    }

    private void TrySubscribe(BaseEnemyHP hp)
    {
        if (hp == null || _subs.Contains(hp)) return;
        hp.OnKilled += OnEnemyKilled;
        _subs.Add(hp);
    }
    
    private void OnEnemyKilled(DamageContext ctx)
    {
        Debug.Log(ctx.Attacker.name);
        Debug.Log(Context.name);
        if (ctx.Attacker != Context) return;

        // именно спец-слот?
        if (ctx.Slot != SkillSlot.Special) return;

        var hp = Context.Hp;
        if (hp == null) return;

        float delta = _maxHpPerKill;
        Debug.Log(hp.MaxHP);
        hp.MaxHP += delta;
        hp.UpdateHP();
        Debug.Log(hp.MaxHP);
        if (_healByGain) hp.ReceiveHP(delta);
        // Debug.Log($"[Arctic Assault] +{delta} Max HP → {hp.MaxHP}");
    }
}
