/*
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BaseEnemyHP), typeof(BaseEnemyAttack))]
public sealed class IfritFervorStatus : MonoBehaviour
{
    private float _threshold;
    private float _lifeTime;
    private float _dmgBonusPct;
    private PlayerContext _ctx;
    private BaseEnemyHP _hp;
    private BaseEnemyAttack _atk;
    private BaseEnemyMove _move;
    private bool _frenzied;
    public void Init(float thr, float life, float bonus, PlayerContext ctx)
    {
        _threshold = thr;
        _lifeTime = life;
        _dmgBonusPct = bonus;
        _ctx = ctx;

        _hp = GetComponent<BaseEnemyHP>();
        _atk = GetComponent<BaseEnemyAttack>();
        _move = GetComponent<BaseEnemyMove>();

        StartCoroutine(CoWatch());
    }

    private IEnumerator CoWatch()
    {
        float t = 0f;
        
        while (t < _lifeTime && _hp.CurrentHP > _hp.MaxHP * _threshold)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (_hp.CurrentHP <= _hp.MaxHP * _threshold)
            Frenzy();
        
        Destroy(this, _lifeTime);
    }
    
    private void Frenzy()
    {
        if (_frenzied) return;
        _frenzied = true;
        
        _atk.AddOutgoingModifier(MultiplyDamage);
        _move.SetTargetingMode(TargetingMode.EnemyOnly);
        _atk.OnDamageApplied += Spread;
    }

    
    private void MultiplyDamage(ref float dmg) => dmg += dmg * _dmgBonusPct;

    private void Spread(IDamageable victim)
    {
        if (victim is not MonoBehaviour mb) return;
        if (mb.TryGetComponent<IfritFervorStatus>(out _)) return;

        var st = mb.gameObject.AddComponent<IfritFervorStatus>();
        st.Init(_threshold, _lifeTime, _dmgBonusPct, _ctx);
    }
}
*/
