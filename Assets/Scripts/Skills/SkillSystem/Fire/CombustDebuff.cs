using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BaseEnemyHP))]
public class CombustDebuff : MonoBehaviour
{
    public bool IsAmplifyPhase  { get; private set; }
    public bool IsActive { get; private set; }
    
    private float _explosionDmg;
    private float _explosionRad;
    private GameObject _vfxPrefab;
    private ActorContext _ownerCtx;
    private Coroutine _lifeRoutine;
    private GameObject _vfxInst;
    private BaseEnemyHP _hp;
    private bool _exploded = false;

    public bool Activate(float duration, float explosionDmg, float explosionRad, GameObject vfxPrefab, ActorContext ownerCtx)
    {
        if (IsActive) return false;
        IsActive = true;
        IsAmplifyPhase = true;

        _explosionDmg = explosionDmg;
        _explosionRad = explosionRad;
        _vfxPrefab = vfxPrefab;
        _ownerCtx = ownerCtx;

        if (_hp == null) _hp = GetComponent<BaseEnemyHP>();

        if (_vfxPrefab)
        {
            _vfxInst = Instantiate(_vfxPrefab, transform);
            _vfxInst.transform.localPosition = Vector3.zero;
        }

        _lifeRoutine = StartCoroutine(LifeTimer(duration));
        return true;
    }

    private IEnumerator LifeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsAmplifyPhase = false;

        Explode();
        ResetState();
    }
    
    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;

        Vector3 p = transform.position;

        var selfCtx = new DamageContext
        {
            Attacker = _ownerCtx,
            Target   = _hp,
            Type     = SkillDamageType.Basic,
            Damage   = _explosionDmg,
            IsCrit   = false,
            CritMultiplier = 1f,
            HitPoint = p,
            SourceGO = gameObject,
            Slot     = SkillSlot.Ultimate
        };
        _ownerCtx.ApplyDamageContextModifiers(ref selfCtx);
        _hp.ReceiveDamage(selfCtx);

        var hits = Physics.OverlapSphere(p, _explosionRad);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].TryGetComponent(out BaseEnemyHP enemy) || enemy == _hp) continue;

            var aoeCtx = new DamageContext
            {
                Attacker = _ownerCtx,
                Target   = enemy,
                Type     = SkillDamageType.Basic,
                Damage   = _explosionDmg,
                IsCrit   = false,
                CritMultiplier = 1f,
                HitPoint = p,
                SourceGO = gameObject,
                Slot     = SkillSlot.Undefined
            };
            _ownerCtx?.ApplyDamageContextModifiers(ref aoeCtx);
            enemy.ReceiveDamage(aoeCtx);
        }
    }


    
    private void ResetState()
    {
        if (_lifeRoutine != null)
        {
            StopCoroutine(_lifeRoutine);
            _lifeRoutine = null;
        }

        IsActive = false;
        IsAmplifyPhase = false;

        if (_vfxInst) Destroy(_vfxInst);
    }

    private void OnDisable() => ResetState();
}
